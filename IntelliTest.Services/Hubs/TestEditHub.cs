using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models.Questions;
using IntelliTest.Core.Models.Tests;
using IntelliTest.Core.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using TiktokenSharp;
using static System.Net.Mime.MediaTypeNames;

namespace IntelliTest.Core.Hubs
{
    public class TestEditHub : Hub
    {
        private readonly IConfiguration config;
        private readonly ILessonService lessonService;

        public TestEditHub(IConfiguration _config, LessonService _lessonService)
        {
            config = _config;
            lessonService = _lessonService;
        }

        private string Translate(string text)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = @"C:\Users\raian\AppData\Local\Programs\Python\Python38\python.exe";
            start.Arguments = string.Format("translate.py \"{0}\"", text);
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            string last = "";
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    last = reader.ReadToEnd();
                }
            }

            //last = last.Substring(2, last.Length - 5);
            //DateTime end = DateTime.Now;
            //string[] splitted = last.Split("*");
            //string all = "";
            //for (int i = 0; i < splitted.Length; i++)
            //{
            //    all += (char)int.Parse(splitted[i]);
            //}

            //var res = all.Split("&").Select(qa => qa.Split("|"));
            return last;
        }

        public async Task FromLesson(string name)
        {
            string text = "";
            string guidString = name.Split('/').Last();
            Guid guidResult;
            bool isValid = Guid.TryParse(guidString, out guidResult);
            if (isValid)
            {
                var lesson = await lessonService.GetById(guidResult);
                if (lesson.IsPrivate)
                {
                    await Clients.Caller.SendAsync("WrongLesson");
                    return;
                }
                text = lesson.Content;
            }
            else
            {
                var lesson = await lessonService.GetByName(name)!;
                if (lesson == null)
                {
                    await Clients.Caller.SendAsync("WrongLesson");
                    return;
                }

                text = lesson.Content;
            }
            await AddQuestion(text);
        }

        public async Task AddQuestion(string prompt)
        {
            //int count = 0;
            //var api = new OpenAI_API.OpenAIAPI(config["OpenAIKey"]);
            //var chat = api.Chat.CreateConversation();

            //string translatedText = Translate(prompt);

            //TikToken tikToken = TikToken.EncodingForModel("gpt-3.5-turbo");
            //var encoded = tikToken.Encode(translatedText);
            //int[] encodedSentenceEnds = new[]
            //{
            //    13, 382, 497,662,627,1131,2564,3343,4527,6058,6389,7609,
            //};

            //int i = 0;
            //List<OpenQuestionViewModel> questions = new List<OpenQuestionViewModel>();
            //while (i < encoded.Count)
            //{
            //    var part = encoded.Skip(i).Take(2000).ToArray();
            //    i += part.Length;
            //    //Not last piece
            //    if (part.Length == 2000)
            //    {
            //        while (!encodedSentenceEnds.Contains(part[i - 1]))
            //        {
            //            i--;
            //        }
            //    }
            //    string text = "Generate questions and answers only on the text only in bulgarian. " + tikToken.Decode(part.Take(i).ToList());
            //    chat.AppendUserInput(text);

            //    Console.WriteLine(tikToken.Encode(text).Count + 2);
            //    List<Tuple<string, string>> final = new List<Tuple<string, string>>();
            //    var res = "";
            //    string question = "";
            //    await foreach (var response in chat.StreamResponseEnumerableFromChatbotAsync())
            //    {
            //        res += response;
            //        if (res.Contains("\n"))
            //        {
            //            if (res.Length < 5)
            //            {
            //                res = res.Replace("\n", "");
            //            }
            //            else if (question == "")
            //            {
            //                question = res;
            //                res = "";
            //            }
            //            else
            //            {
            //                await Clients.Caller.SendAsync("Add", question, res, count);
            //                count++;
            //                res = "";
            //            }
            //        }
            //    }
            //    var lines = res.Replace("\r", "").Split("\n");
            //    for (int j = 2; j < lines.Length; j += 2)
            //    {
            //        questions.Add(new OpenQuestionViewModel()
            //        {
            //            Order = count,
            //            Text = lines[j - 2],
            //            Answer = lines[j - 1]
            //        });
            //        count++;
            //    }
            //}
            Thread.Sleep(3000);
            for (int i = 0; i < 5; i++)
            {
                Thread.Sleep(1000);
                await Clients.Caller.SendAsync("Add", "Who am i", "You", i);
            }
        }
    }
}
