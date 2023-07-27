using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models.Classes;
using IntelliTest.Core.Services;
using IntelliTest.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace IntelliTest.Areas.Admin.Controllers
{
    public class ClassesController : AdminController
    {
        private readonly IClassService classService;
        //private readonly IDistributedCache cache;
        private readonly IMemoryCache cache;


        public ClassesController(IClassService _classService, IMemoryCache _cache)
        {
            classService = _classService;
            cache = _cache;
        }
        public async Task<IActionResult> Index()
        {
            if (cache.TryGetValue("adminClasses", out IEnumerable<ClassViewModel>? model))
            {
            }
            else
            {
                model = await classService.GetAllAdmin();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));
                cache.SetAsync("adminClasses", model, cacheEntryOptions);
            }
            return View(model);
        }
    }
}
