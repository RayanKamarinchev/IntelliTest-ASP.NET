using AutoMapper;
using Google;
using IntelliTest.Core.Hubs;
using IntelliTest.Core.Models.Chat;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;
using IntelliTest.Core.Contracts;
using IntelliTest.Data;
using Microsoft.EntityFrameworkCore;
using IntelliTest.Core.Services;

namespace IntelliTest.Controllers
{
    public class MessagesController : Controller
    {
        private readonly IMessageService messageService;
        public MessagesController(IMessageService _messageService)
        {
            messageService = _messageService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(Guid id)
        {
            throw new NotImplementedException();
        }

        [HttpGet("Room/{roomName}")]
        public IActionResult GetMessages(string roomName)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<ActionResult> Create(MessageViewModel viewModel)
        {
            throw new NotImplementedException();
        // return CreatedAtAction(nameof(Get), new { id = msg.Id }, createdMessage);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
