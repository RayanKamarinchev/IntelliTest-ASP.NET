using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models;
using IntelliTest.Core.Services;
using IntelliTest.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace IntelliTest.Controllers
{
    public class HomeController : Controller
    {
        public HomeController(ITeacherService teacherService, IStudentService studentService)
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
        public IActionResult Index()
        {
            return View();
        }
        [Route("/Error/{statusCode}")]
        public IActionResult Error(int statusCode)
        {
            string message = "Опа... Нещо се обърка";
            switch (statusCode)
            {
                case 401:
                    message = "Отказан достъп!";
                    break;
                case 404:
                    message = "Съдържанието което търсите не е намерено.";
                    break;
                case 500:
                    message = "Грешка в сървъра";
                    break;
            }
            return View("Error",new ErrorViewModel
            {
                StatusCode = statusCode,
                Message = message
            });
        }
    }
}