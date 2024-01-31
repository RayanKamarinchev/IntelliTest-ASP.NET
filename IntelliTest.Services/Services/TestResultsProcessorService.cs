using System.Text.RegularExpressions;
using IntelliTest.Core.Models;
using IntelliTest.Data;
using IntelliTest.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using TiktokenSharp;
using JsonSerializer = System.Text.Json.JsonSerializer;

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

        private async Task<Tuple<float, string>> CalculateOpenQuestionScore(string Answer, string RightAnswer, float MaxScore)
        {
            if (Answer is null)
            {
                return new Tuple<float, string>(0, $"Правилният отговор е \"{RightAnswer}\"");
            }
            if (Regex.Matches(RightAnswer, @"\w+")
                     .Select(m => m.Value).Count() <= 4)
            {
                bool isCorrect = Regex.Matches(Answer, @"\w+")
                                      .Select(m => m.Value)
                                      .SequenceEqual(Regex.Matches(RightAnswer, @"\w+")
                                                          .Select(m => m.Value));
                return new Tuple<float, string>(isCorrect ? MaxScore : 0, $"Правилният отговор е \"{RightAnswer}\"");
            }
            var api = new OpenAI_API.OpenAIAPI(apiKey);
            var chat = api.Chat.CreateConversation();

            TikToken tikToken = TikToken.EncodingForModel("gpt-3.5-turbo");
            var encodedCorrect = tikToken.Encode(RightAnswer);
            var partOfCorrect = encodedCorrect.Take(2000).ToList();
            var encodedAnswer = tikToken.Encode(Answer);
            var partOfAnswer = encodedAnswer.Take(2000).ToList();
            string prompt = "Correct answer \"" + tikToken.Decode(partOfCorrect) + "\" Given answer \"" + tikToken.Decode(partOfAnswer) + $"\" Answer with json containing score out of {MaxScore*2} and message in bulgarian";
            chat.AppendUserInput(prompt);
            var responseJson = await chat.GetResponseFromChatbotAsync();
            Response? response = JsonSerializer.Deserialize<Response>(responseJson);
            float score = response.score / 2;
            string message = response.message;
            return new Tuple<float, string>(score, message);
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
