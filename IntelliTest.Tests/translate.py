from googletrans import Translator
import sys
translator = Translator()
print(translator.translate(sys.argv[1], src="bg").text)