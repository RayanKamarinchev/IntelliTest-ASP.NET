using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models.Chat;
using IntelliTest.Core.Services;
using IntelliTest.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntelliTest.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IRoomService roomService;
        public ChatController(IRoomService _roomService)
        {
            roomService = _roomService;
        }

        [Route("Chat")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await roomService.GetChatsViewModel(User.Id());
            return View(model);
        }
    }
}
