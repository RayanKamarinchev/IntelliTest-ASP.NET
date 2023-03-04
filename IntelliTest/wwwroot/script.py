# coding=utf8
import pke
import string
import re
import nltk
# nltk.download('stopwords')
# nltk.download('brown')
from nltk.tokenize import sent_tokenize
from nltk.corpus import stopwords
import time
import spacy
from sense2vec import Sense2Vec
from nltk import FreqDist
from nltk.corpus import brown
from similarity.normalized_levenshtein import NormalizedLevenshtein
from torch import nn
import torch
from googletrans import Translator
from transformers import AutoModelWithLMHead, AutoTokenizer

import datefinder

class BgQuestion(nn.Module):
    def is_far(self, words_list,currentword,thresh,normalized_levenshtein):
        if sum(1 for c in currentword if c.isupper()) / (currentword.count(" ")+1) > 0.5:
            return True
        threshold = thresh
        score_list =[]
        for word in words_list:
            score_list.append(normalized_levenshtein.distance(word.lower(),currentword.lower()))
        if min(score_list)>=threshold:
            return True
        else:
            return False

    def MCQs_available(self, word,s2v):
        word = word.replace(" ", "_")
        if sum(1 for c in word if c.isupper()) > 0:
            return True
        sense = s2v.get_best_sense(word)
        if sense is not None:
            return True
        else:
            return False

    def get_phrases(self, doc):
        phrases={}
        for np in doc.noun_chunks:
            phrase =np.text
            len_phrase = len(phrase.split())
            if len_phrase > 1:
                if phrase not in phrases:
                    phrases[phrase]=1
                else:
                    phrases[phrase]=phrases[phrase]+1

        phrase_keys=list(phrases.keys())
        phrase_keys = sorted(phrase_keys, key= lambda x: len(x),reverse=True)
        phrase_keys=phrase_keys[:50]
        return phrase_keys

    def filter_phrases(self, phrase_keys,max,normalized_levenshtein ):
        filtered_phrases =[]
        if len(phrase_keys)>0:
            filtered_phrases.append(phrase_keys[0])
            for ph in phrase_keys[1:]:
                if self.is_far(filtered_phrases,ph,0.7,normalized_levenshtein ):
                    filtered_phrases.append(ph)
                if len(filtered_phrases)>=max:
                    break
        return filtered_phrases

    def get_nouns_multipartite(self, text):
        out = []

        extractor = pke.unsupervised.MultipartiteRank()
        extractor.load_document(input=text, language='en')
        pos = {'PROPN', 'NOUN'}
        stoplist = list(string.punctuation)
        stoplist += stopwords.words('english')
        extractor.candidate_selection(pos=pos)
        # 4. build the Multipartite graph and rank candidates using random walk,
        #    alpha controls the weight adjustment mechanism, see TopicRank for
        #    threshold/method parameters.
        extractor.candidate_weighting(alpha=1.1,
                                      threshold=0.75,
                                      method='average')

        keyphrases = extractor.get_n_best(n=100)

        for key in keyphrases:
            out.append(key[0])

        return out, extractor.weights

    def tokenize_sentences(self, text):
        sentences = [sent_tokenize(text)]
        sentences = [y for x in sentences for y in x]
        # Remove any short sentences less than 20 letters.
        sentences = [sentence.strip() for sentence in sentences if len(sentence) > 20]
        return sentences

    def get_keywords(self, nlp,text,max_keywords,s2v,fdist,normalized_levenshtein,no_of_sentences):
        doc = nlp(text)
        max_keywords = int(max_keywords)
        keywords, weights = self.get_nouns_multipartite(text)
        keywords = sorted(keywords, key=lambda x: fdist[x])
        keywords = self.filter_phrases(keywords, max_keywords,normalized_levenshtein )

        phrase_keys = self.get_phrases(doc)

        filtered_phrases = self.filter_phrases(phrase_keys, max_keywords,normalized_levenshtein )

        dates = datefinder.find_dates(text, False, True)
        dateRanges = [(d[1][0]+1, d[1][1]-2) for d in dates]
        datesText = [text[d[0]: d[1]] for d in dateRanges]
        nums = []
        indexes = re.finditer(r'\d+', text)
        for i in indexes:
            isDate = False
            for d in dateRanges:
                if d[0] <= i.start() <= d[1]:
                    isDate = True
            if not isDate:
                nums.append(text[i.start():i.end()])

        total_phrases = keywords + filtered_phrases + datesText + nums

        total_phrases_filtered = self.filter_phrases(total_phrases, min(max_keywords, 2*no_of_sentences),normalized_levenshtein )

        answers = []
        for answer in total_phrases_filtered:
            if answer not in answers and self.MCQs_available(answer,s2v):
                answers.append(answer)

        answers = answers[:max_keywords]
        return answers, weights

    def get_question(self, answer, context,tokenizer,model, max_length=64):
        input_text = f"answer: {answer}  context: {context} </s>"
        features = tokenizer([input_text], return_tensors='pt')

        output = model.generate(input_ids=features['input_ids'],
                                attention_mask=features['attention_mask'],
                                max_length=max_length)

        return tokenizer.decode(output[0])
    def __init__(self):
        super().__init__()
        self.nlp = spacy.load('en_core_web_sm')
        self.s2v = Sense2Vec()
        self.dist = FreqDist(brown.words())
        self.normalized_levenshtein = NormalizedLevenshtein()
        self.translator = Translator()
        self.tokenizer = AutoTokenizer.from_pretrained("mrm8488/t5-base-finetuned-question-generation-ap")
        self.model = AutoModelWithLMHead.from_pretrained("mrm8488/t5-base-finetuned-question-generation-ap")

    def forward(self, bg_text):
        translated_text = self.translator.translate(bg_text, src="bg").text
        modified_text = " ".join(self.tokenize_sentences(translated_text))
        words, weights = self.get_keywords(self.nlp, modified_text, 10000, self.s2v, self.dist, self.normalized_levenshtein, 10000)
        questions = []
        for answer in words:
            q = self.get_question(answer, modified_text, self.tokenizer, self.model)
            q_translated = self.translator.translate(q, dest="bg").text
            answer = self.translator.translate(answer, dest="bg").text
            questions.append([q_translated, answer])
        q_out = []
        for q in questions:
            q_out.append("|".join(q))
        qi = "&".join(q_out)
        r = [str(ord(q)) for q in qi]
        return "*".join(r)

import sys
model = BgQuestion()
model.eval()
r = model("".join(sys.argv[1]))
print(r)
