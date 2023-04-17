using IntelliTest.Core.Models.Chat;
using IntelliTest.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using IntelliTest.Core.Contracts;
using IntelliTest.Core.Services;
using IntelliTest.Services.Infrastructure;
using Microsoft.AspNetCore.Authorization;

namespace IntelliTest.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : Controller
    {
        private readonly IMessageService messageService;

        public MessagesController(IMessageService _messageService)
        {
            messageService = _messageService;
        }
        [HttpGet("Get/{id}")]
        public async Task<ActionResult<Room>> Get(Guid id)
        {
            var messageViewModel = await messageService.GetById(id);
            if (messageViewModel == null)
            {
                return NotFound();
            }
            return Ok(messageViewModel);
        }

        [HttpGet("Room/{roomName}")]
        public async Task<IActionResult> GetMessages(string roomName)
        {
            var messagesViewModel = await messageService.GetMessages(roomName);
            if (messagesViewModel == null)
            {
                return BadRequest();
            }
            return Ok(messagesViewModel);
        }

        [HttpGet("Create")]
        public async Task<IActionResult> Create(string content, string room)
        {
            var createdMessage = await messageService.Create(new MessageViewModel()
            {
                Content = content,
                Room = room
            }, User.Id());
            if (createdMessage == null)
            {
                return BadRequest();
            }
            return CreatedAtAction(nameof(Get), new { id = createdMessage.Id }, createdMessage);
        }

        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            bool res = await messageService.Delete(id, User.Id());
            if (!res)
            {
                return NotFound();
            }
            return Ok();
        }
    }
}
