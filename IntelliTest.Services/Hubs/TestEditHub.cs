﻿using System;
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

        public TestEditHub(IConfiguration _config, ILessonService _lessonService)
        {
            config = _config;
            lessonService = _lessonService;
        }

        public async Task FromLesson(string name, int questionCount)
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
            await AddQuestion(text, questionCount);
        }

        public async Task AddQuestion(string prompt, int questionCount)
        {
            var api = new OpenAI_API.OpenAIAPI(config["OpenAIKey"]);
            var chat = api.Chat.CreateConversation();
            TikToken tikToken = TikToken.EncodingForModel("gpt-3.5-turbo");
            var encoded = tikToken.Encode(prompt);
            int[] encodedSentenceEnds = new[]
            {
                13, 382, 497,662,627,1131,2564,3343,4527,6058,6389,7609,
            };

            int i = 0;
            while (i < encoded.Count)
            {
                var part = encoded.Skip(i).Take(2000).ToArray();
                i += part.Length;
                //Not last piece
                if (part.Length == 2000)
                {
                    while (!encodedSentenceEnds.Contains(part[i - 1]))
                    {
                        i--;
                    }
                }

                string decodedText = tikToken.Decode(part.Take(i).ToList());
                string text =
                    "Generate questions \"Q\" and answers \"A\" only in Bulgarian, focusing exclusively on the content of the following text: '''" + decodedText + "'''.Ensure the questions and answers can be found in the text. Generate " + questionCount + " questions without enumerating them.";
                chat.AppendUserInput(text);

                var res = "";
                string question = "";
                await foreach (var response in chat.StreamResponseEnumerableFromChatbotAsync())
                {
                    res += response;
                    if (res.Contains("\n"))
                    {
                        if (res.Length < 5)
                        {
                            res = res.Replace("\n", "");
                        }
                        else if (question == "")
                        {
                            if (res.StartsWith("Q:"))
                            {
                                res = res.Substring(3, res.Length - 3);
                            }
                            question = res.Replace("\n", "");
                            res = "";
                        }
                        else
                        {
                            if (res.StartsWith("A:"))
                            {
                                res = res.Substring(3, res.Length - 3);
                            }
                            res = res.Replace("\n", "");
                            await Clients.Caller.SendAsync("Add", question, res);
                            res = "";
                            question = "";
                        }
                    }
                }

                res = res.Substring(3, res.Length - 3).Replace("\n", "");
                await Clients.Caller.SendAsync("Add", question, res);
            }
        }
    }
}
