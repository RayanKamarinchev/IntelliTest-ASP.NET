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
        public ChatController(ITeacherService teacherService, IStudentService studentService)
        {
            if (!TempData.Keys.Contains("TeacherId"))
            {
                TempData["TeacherId"] = teacherService.GetTeacherId(User.Id());
            }
            if (!TempData.Keys.Contains("StudentId"))
            {
                TempData["StudentId"] = studentService.GetStudentId(User.Id());
            }
        }
        [HttpGet("Index")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
