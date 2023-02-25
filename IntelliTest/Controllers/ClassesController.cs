using Microsoft.AspNetCore.Mvc;

namespace IntelliTest.Controllers
{
    public class ClassesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
