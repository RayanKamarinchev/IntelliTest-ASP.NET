using Microsoft.AspNetCore.Mvc;

namespace IntelliTest.Areas.Admin.Controllers
{
    public class HomeController : AdminController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
