using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models.Mails;
using IntelliTest.Core.Models.Tests;
using IntelliTest.Core.Models.Users;
using IntelliTest.Core.Models;
using IntelliTest.Data.Entities;
using IntelliTest.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IntelliTest.Areas.Admin.Controllers
{
    public class UserController : AdminController
    {
        private readonly SignInManager<User> signInManager;

        public UserController(UserManager<User> _userManager,
                              SignInManager<User> _signInManager,
                              RoleManager<IdentityRole> _roleManager,
                              ITeacherService _teacherService,
                              IStudentService _studentService,
                              ILessonService _lessonService,
                              IEmailService email_service,
                              ITestService testService,
                              IWebHostEnvironment webHostEnvironment,
                              ITestResultsService testResultsService)
        {
            signInManager = _signInManager;
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            TempData.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
