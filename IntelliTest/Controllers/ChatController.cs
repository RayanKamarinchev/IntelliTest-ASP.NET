using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntelliTest.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IMessageService messageService;
        public ChatController(IMessageService _messageService)
        {
            messageService = _messageService;
        }
        [HttpGet("Index")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
