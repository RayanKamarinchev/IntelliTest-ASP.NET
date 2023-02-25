using Microsoft.AspNetCore.Mvc;

namespace IntelliTestWeb.Controllers
{
    public class TestsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
