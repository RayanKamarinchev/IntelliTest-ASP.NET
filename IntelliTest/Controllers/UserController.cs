using System.Security.Claims;
using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models;
using IntelliTest.Core.Models.Mails;
using IntelliTest.Core.Models.Tests;
using IntelliTest.Core.Models.Users;
using IntelliTest.Data.Entities;
using IntelliTest.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IntelliTest.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ITeacherService teacherService;
        private readonly IStudentService studentService;
        private readonly ILessonService lessonService;
        private readonly IEmailService emailService;
        private readonly ITestService testService;
        private readonly ITestResultsService testResultsService;
        private readonly IWebHostEnvironment webHostEnvironment;

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
            userManager = _userManager;
            signInManager = _signInManager;
            studentService = _studentService;
            teacherService = _teacherService;
            lessonService = _lessonService;
            this.emailService = email_service;
            roleManager = _roleManager;
            this.testService = testService;
            this.webHostEnvironment = webHostEnvironment;
            this.testResultsService = testResultsService;
        }

        private string GetEmailTemplate(string link)
        {
            string path = webHostEnvironment.WebRootPath + "/html/email.html";
            string emailHtml = System.IO.File.ReadAllText(path);
            emailHtml = emailHtml.Replace("$$$", link);
            return emailHtml;
        }

        private async Task GiveRole(User user, string roleName)
        {
            var roleExists = await roleManager.RoleExistsAsync(roleName);
            if (roleExists)
            {
                await userManager.AddToRoleAsync(user, roleName);
            }
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
                return RedirectToAction("Login");
            }

            var user = await userManager.FindByNameAsync(model.Username);

            if (user != null)
            {
                var res = await signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                if (res.Succeeded)
                {
                    AddRoleIdsToTempData(user.Id);
                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("", "Invalid Login");
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            TempData.Clear();
            return RedirectToAction("Index", "Home");
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ForgotPassword()
        {
            return View("ForgotPassword", "");
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var link = Url.Action("ResetPassword", "User", new { token, email = user.Email }, Request.Scheme);
                var message = new EmailMessage(email, "Password reset", GetEmailTemplate(link));
                bool success = await emailService.SendAsync(message, new CancellationToken());
                if (success)
                {
                    TempData["message"] = "Имейлът е изпратен";
                }
                else
                {
                    TempData["message"] = "Грешка! Нещо се обърка";
                }
            }

            return RedirectToAction("Login");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            return View(new ResetPassword()
            {
                Email = email,
                Token = token
            });
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPassword model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var resetPassword = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
                if (!resetPassword.Succeeded)
                {
                    foreach (var resetPasswordError in resetPassword.Errors)
                    {
                        ModelState.AddModelError(resetPasswordError.Code, resetPasswordError.Description);
                    }
                    TempData["message"] = "Нeщо се обърка!";
                }
                else
                {
                    TempData["message"] = "Паролата е сменена";
                }
            }

            return RedirectToAction("Login");
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
                IsTeacher = User.IsTeacher(),
                ImageUrl = user.PhotoPath
            };
            TempData["imagePath"] = user.PhotoPath;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ViewProfile(EditUser model)
        {
            var user = await userManager.GetUserAsync(User);
            model.ImageUrl = (string)TempData.Peek("imagePath");
            if (model.Image != null && model.Image.ContentType.StartsWith("image"))
            {
                string folder = "imgs/";
                if (string.IsNullOrEmpty((string)TempData.Peek("imagePath")))
                {
                    folder += Guid.NewGuid() + "_" + model.Image.FileName;
                    model.ImageUrl = folder;
                }
                else
                {
                    folder = (string)TempData["imagePath"];
                }
                string serverFolder = Path.Combine(webHostEnvironment.WebRootPath, folder);
                await model.Image.CopyToAsync(new FileStream(serverFolder, FileMode.Create));
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                user.PhotoPath = folder;
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                if (model.Password != null && await userManager.CheckPasswordAsync(user, model.Password))
                {
                    user.Email = model.Email;
                }

                if (!User.IsTeacher() && !User.IsStudent())
                {
                    if (model.IsTeacher)
                    {
                        await GiveRole(user, "Teacher");
                        await teacherService.AddTeacher(User.Id(), model.School);
                    }
                    else
                    {
                        await GiveRole(user, "Student");
                        await studentService.AddStudent(new UserType()
                        {
                            Grade = 0,
                            IsStudent = true,
                            School = ""
                        }, User.Id());
                    }

                    return await Logout();
                }
            }

            await testService.SaveChanges();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetPanel(string type)
        {
            switch (type)
            {
                case "results":
                    var results = await testResultsService.GetStudentsTestsResults((Guid)TempData.Peek("StudentId"));
                    return PartialView("Panels/UserTestResultsPartialView", results);
                case "read":
                    var read = await lessonService.ReadLessons(User.Id());
                    return PartialView("Panels/ReadLikedLeassonsPartialView", read);
                case "like":
                    var liked = await lessonService.LikedLessons(User.Id());
                    return PartialView("Panels/ReadLikedLeassonsPartialView", liked);
                case "myTests":
                    QueryModel<TestViewModel> myTests = new QueryModel<TestViewModel>();
                    if (User.IsTeacher())
                    {
                        myTests = await testService.GetMy((Guid)TempData.Peek("TeacherId"), null, new QueryModel<TestViewModel>());
                    }
                    else if (User.IsStudent())
                    {
                        Guid studentOwnerId = (Guid)TempData.Peek("StudentId");
                        myTests = await testService.TestsTakenByStudent(studentOwnerId, new QueryModel<TestViewModel>());
                    }

                    return PartialView("Panels/MyTestsPartialView", myTests);
                default:
                    var user = await userManager.GetUserAsync(User);
                    EditUser model = new EditUser()
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        IsTeacher = User.IsTeacher()
                    };
                    return PartialView("Panels/UserInfoPartialView", model);
            }
        }

        [HttpGet]
        public async Task<bool> CheckPassword(string password)
        {
            var user = await userManager.GetUserAsync(User);
            bool res = await userManager.CheckPasswordAsync(user, password);
            return res;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action("ExternalLoginCallback", "User");
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
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
                var user = await userManager.FindByEmailAsync(info.Principal.FindFirstValue(ClaimTypes.Email));
                AddRoleIdsToTempData(user.Id);
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
                    AddRoleIdsToTempData(user.Id);
                    return RedirectToAction("Index", "Home");
                }
                return RedirectToAction("Register");
            }
        }

        public void AddRoleIdsToTempData(string userId)
        {
            if (!TempData.Keys.Contains("TeacherId"))
            {
                TempData["TeacherId"] = teacherService.GetTeacherId(userId);
            }
            if (!TempData.Keys.Contains("StudentId"))
            {
                TempData["StudentId"] = studentService.GetStudentId(userId);
            }
        }
    }
}
