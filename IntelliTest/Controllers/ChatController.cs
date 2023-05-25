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
        [HttpGet("Index")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
