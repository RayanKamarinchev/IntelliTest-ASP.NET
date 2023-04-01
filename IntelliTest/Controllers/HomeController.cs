using IntelliTest.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Watchlist.Controllers
{
    public class HomeController : Controller
    {
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