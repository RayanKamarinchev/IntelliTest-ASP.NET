using System.Text.RegularExpressions;
using IntelliTest.Data;
using IntelliTest.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using TiktokenSharp;
using static IntelliTest.Core.Objects.Translator;

namespace IntelliTest.Core.Services
{
    public class TestResultsProcessorService : BackgroundService
    {
        private readonly IntelliTestDbContext context;
        private readonly string apiKey;
        private List<OpenQuestionAnswer> openQuestionAnswers;

        public TestResultsProcessorService(string apiKey,string connectionString, IntelliTestDbContext _context)
        {
            this.apiKey = apiKey;
            var optionsBuilder = new DbContextOptionsBuilder<IntelliTestDbContext>();
            optionsBuilder.UseSqlServer(connectionString);
            context = new IntelliTestDbContext(optionsBuilder.Options);
        }
        public void setOpenQuesitons(List<OpenQuestionAnswer> openQuestionAnswers)
        {
            this.openQuestionAnswers = openQuestionAnswers;
        }

        private async Task<Tuple<decimal, string>> CalculateOpenQuestionScore(string Answer, string RightAnswer, int MaxScore)
        {
            if (Answer is null)
            {
                return new Tuple<decimal, string>(0, $"Правилният отговор е \"{RightAnswer}\"");
            }
            if (Regex.Matches(RightAnswer, @"\w+")
                     .Select(m => m.Value).Count() <= 4)
            {
                bool isCorrect = Regex.Matches(Answer, @"\w+")
                                      .Select(m => m.Value)
                                      .SequenceEqual(Regex.Matches(RightAnswer, @"\w+")
                                                          .Select(m => m.Value));
                return new Tuple<decimal, string>(isCorrect ? MaxScore : 0, $"Правилният отговор е \"{RightAnswer}\"");
            }
            var api = new OpenAI_API.OpenAIAPI(apiKey);
            var chat = api.Chat.CreateConversation();

            string translatedCorrectAnswer = Translate(RightAnswer);
            string translatedAnswer = Translate(Answer);

            TikToken tikToken = TikToken.EncodingForModel("gpt-3.5-turbo");
            var encodedCorrect = tikToken.Encode(translatedCorrectAnswer);
            var partOfCorrect = encodedCorrect.Take(2000).ToList();
            var encodedAnswer = tikToken.Encode(translatedAnswer);
            var partOfAnswer = encodedAnswer.Take(2000).ToList();
            string text = "Correct answer \"" + tikToken.Decode(partOfCorrect) + "\" Given answer \"" + tikToken.Decode(partOfAnswer) + $"Answer with \"Score: (score out of {MaxScore * 2}) Message: (message in bulgarian)\"";
            chat.AppendUserInput(text);
            var response = await chat.GetResponseFromChatbotAsync();
            decimal score = decimal.Parse(Regex.Match(response, @"\d+").Value) / 2;
            string message = response.Substring(response.IndexOf(':', response.IndexOf(':') + 3))
                                     .Replace('\n', ' ');
            return new Tuple<decimal, string>(score, message);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            foreach (var q in openQuestionAnswers)
            {
                var eval =
                    await CalculateOpenQuestionScore(q.Answer, q.Question.Answer, q.Question.MaxScore);
                q.Points = eval.Item1;
                q.Explanation = eval.Item2;
            }
            
            context.UpdateRange(openQuestionAnswers);
            await context.SaveChangesAsync();
        }
    }
}
