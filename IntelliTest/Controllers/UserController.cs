using IntelliTest.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models.Users;
using IntelliTest.Services.Infrastructure;

namespace Watchlist.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly ITeacherService teacherService;
        private readonly IStudentService studentService;
        private readonly ILessonService lessonService;


        public UserController(UserManager<User> _userManager,
                              SignInManager<User> _signInManager,
                              ITeacherService _teacherService,
                              IStudentService _studentService,
                              ILessonService _lessonService)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            studentService = _studentService;
            teacherService = _teacherService;
            lessonService = _lessonService;
        }

        public void ClearCookies()
        {
            TempData.Remove("isStudent");
            TempData.Remove("isTeacher");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            var model = new RegisterViewModel();
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new User()
            {
                Email = model.Email,
                UserName = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
            };

            var res = await userManager.CreateAsync(user, model.Password);
            if (res.Succeeded)
            {
                //await signInManager.SignInAsync(user, isPersistent: false);

                return RedirectToAction("Login", "User");
            }

            foreach (var error in res.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            ClearCookies();
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }
            var model = new LoginViewModel();
            model.ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.FindByNameAsync(model.Username);

            if (user != null)
            {
                var res = await signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                if (res.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("", "Invalid Login");
            ClearCookies();
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            ClearCookies();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> ViewProfile()
        {
            var user = await userManager.GetUserAsync(User);
            EditUser model = new EditUser()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                IsTeacher = (bool)TempData.Peek("IsTeacher"),
                HasRole = (bool)TempData.Peek("IsTeacher") || (bool)TempData.Peek("IsStudent")
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ViewProfile(EditUser model)
        {
            var user = await userManager.GetUserAsync(User);
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            if (model.Password!=null && await userManager.CheckPasswordAsync(user, model.Password))
            {
                user.Email = model.Email;
            }

            if (!model.HasRole)
            {
                if (model.IsTeacher)
                {
                    await teacherService.AddTeacher(User.Id());
                    TempData["IsTeacher"] = true;
                }
                else
                {
                    await studentService.AddStudent(new UserType()
                    {
                        Grade = 0,
                        IsStudent = true,
                        School = ""
                    }, User.Id());
                    TempData["isStudent"] = true;
                }

                model.HasRole = true;
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetPanel(string type)
        {
            switch (type)
            {
                case "results":
                    Guid studentId = await studentService.GetStudentId(User.Id());
                    var results = await studentService.GetAllResults(studentId);
                    return PartialView("Panels/UserTestResultsPartialView", results);
                case "read":
                    var read = await lessonService.ReadLessons(User.Id());
                    return PartialView("~/Views/Lessons/Index.cshtml", read);
                case "like":
                    var liked = await lessonService.LikedLessons(User.Id());
                    return PartialView("~/Views/Lessons/Index.cshtml", liked);
                default:
                    var user = await userManager.GetUserAsync(User);
                    EditUser model = new EditUser()
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        IsTeacher = (bool)TempData.Peek("IsTeacher"),
                        HasRole = (bool)TempData.Peek("IsTeacher") || (bool)TempData.Peek("IsStudent")
                    };
                    return PartialView("Panels/UserInfoPartialView", model);
            }
        }

        [HttpGet]
        public async Task<string> ImageUpload(FileUpload file)
        {
            var user = await userManager.GetUserAsync(User);
            using (var ms = new MemoryStream())
            {
                await file.file.CopyToAsync(ms);
                var fileBytes = ms.ToArray();
                user.Photo = fileBytes;
                //save this
            }

            return "Ok";
        }

        [HttpGet]
        public async Task<bool> CheckPassword(string password)
        {
            var user = await userManager.GetUserAsync(User);
            bool res = await userManager.CheckPasswordAsync(user, password);
            return res;
        }


        [HttpGet]
        public async Task<IActionResult> UserType()
        {
            if (await studentService.ExistsByUserId(User.Id()) || await teacherService.ExistsByUserId(User.Id()))
            {
                return BadRequest();
            }
            return View(new UserType());
        }
        [HttpPost]
        public async Task<IActionResult> UserType(UserType model)
        {
            if (await studentService.ExistsByUserId(User.Id()) || await teacherService.ExistsByUserId(User.Id()))
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                View(model);
            }

            if (model.IsStudent)
            {
                await studentService.AddStudent(model, User.Id());
            }
            else
            {
                await teacherService.AddTeacher(User.Id());
            }
            ClearCookies();
            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action("ExternalLoginCallback", "User");
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            ClearCookies();
            return new ChallengeResult(provider, properties);
        }
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string remoteError = null)
        {
            if (remoteError != null)
            {
                TempData["ErrorMessage"] = $"Error from external provider: {remoteError}";
                return RedirectToAction("Login");
            }
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                TempData["ErrorMessage"] = "Error loading external login information.";
                return RedirectToAction("Login");
            }
            
            // Sign in the user with this external login provider if the user already has a login.
            var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                if (email != null)
                {
                    var user = await userManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        user = new User()
                        {
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName),
                            LastName = info.Principal.FindFirstValue(ClaimTypes.Surname)
                        };

                        await userManager.CreateAsync(user);
                    }

                    await userManager.AddLoginAsync(user, info);
                    await signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("Index", "Home");
                }
                return RedirectToAction("Register");
            }
        }
    }
}
