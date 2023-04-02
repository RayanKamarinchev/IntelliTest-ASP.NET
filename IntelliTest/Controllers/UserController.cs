using IntelliTest.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models.Users;
using IntelliTest.Services.Infrastructure;
using Microsoft.AspNetCore.Identity.UI.Services;
using IntelliTest.Core.Models;
using IntelliTest.Core.Models.Mails;
using IntelliTest.Core.Services;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;

namespace Watchlist.Controllers
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

        public UserController(UserManager<User> _userManager,
                              SignInManager<User> _signInManager,
                              RoleManager<IdentityRole> _roleManager,
                              ITeacherService _teacherService,
                              IStudentService _studentService,
                              ILessonService _lessonService,
                              IEmailService email_service)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            studentService = _studentService;
            teacherService = _teacherService;
            lessonService = _lessonService;
            this.emailService = email_service;
            roleManager = _roleManager;
        }

        private string GetEmailTemplate(string link)
        {
            //The template is made with https://my.stripo.email/
            return
                "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\r\n<html xmlns=\"http://www.w3.org/1999/xhtml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\" style=\"font-family:arial,'helvetica neue',helvetica,sans-serif\">\r\n<head>\r\n<link type=\"text/css\" rel=\"stylesheet\" id=\"dark-mode-custom-link\">\r\n<link type=\"text/css\" rel=\"stylesheet\" id=\"dark-mode-general-link\">\r\n<meta charset=\"UTF-8\">\r\n<meta content=\"width=device-width, initial-scale=1\" name=\"viewport\">\r\n<meta name=\"x-apple-disable-message-reformatting\">\r\n<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">\r\n<meta content=\"telephone=no\" name=\"format-detection\">\r\n<title>New message</title><!--[if (mso 16)]>\r\n<style type=\"text/css\">a{text-decoration:none}</style>\r\n<![endif]--><!--[if gte mso 9]><style>sup{font-size:100%!important}</style><![endif]--><!--[if gte mso 9]>\r\n<xml>\r\n<o:OfficeDocumentSettings>\r\n<o:AllowPNG></o:AllowPNG>\r\n<o:PixelsPerInch>96</o:PixelsPerInch>\r\n</o:OfficeDocumentSettings>\r\n</xml>\r\n<![endif]-->\r\n<style type=\"text/css\">#outlook a{padding:0}.es-button{mso-style-priority:100!important;text-decoration:none!important}a[x-apple-data-detectors]{color:inherit!important;text-decoration:none!important;font-size:inherit!important;font-family:inherit!important;font-weight:inherit!important;line-height:inherit!important}.es-desk-hidden{display:none;float:left;overflow:hidden;width:0;max-height:0;line-height:0;mso-hide:all}@media only screen and (max-width:600px){p,ul li,ol li,a{line-height:150%!important}h1,h2,h3,h1 a,h2 a,h3 a{line-height:120%!important}h1{font-size:36px!important;text-align:left}h2{font-size:26px!important;text-align:left}h3{font-size:20px!important;text-align:left}.es-header-body h1 a,.es-content-body h1 a,.es-footer-body h1 a{font-size:36px!important;text-align:left}.es-header-body h2 a,.es-content-body h2 a,.es-footer-body h2 a{font-size:26px!important;text-align:left}.es-header-body h3 a,.es-content-body h3 a,.es-footer-body h3 a{font-size:20px!important;text-align:left}.es-menu td a{font-size:12px!important}.es-header-body p,.es-header-body ul li,.es-header-body ol li,.es-header-body a{font-size:14px!important}.es-content-body p,.es-content-body ul li,.es-content-body ol li,.es-content-body a{font-size:14px!important}.es-footer-body p,.es-footer-body ul li,.es-footer-body ol li,.es-footer-body a{font-size:14px!important}.es-infoblock p,.es-infoblock ul li,.es-infoblock ol li,.es-infoblock a{font-size:12px!important}*[class=\"gmail-fix\"]{display:none!important}.es-m-txt-c,.es-m-txt-c h1,.es-m-txt-c h2,.es-m-txt-c h3{text-align:center!important}.es-m-txt-r,.es-m-txt-r h1,.es-m-txt-r h2,.es-m-txt-r h3{text-align:right!important}.es-m-txt-l,.es-m-txt-l h1,.es-m-txt-l h2,.es-m-txt-l h3{text-align:left!important}.es-m-txt-r img,.es-m-txt-c img,.es-m-txt-l img{display:inline!important}.es-button-border{display:inline-block!important}a.es-button,button.es-button{font-size:20px!important;display:inline-block!important}.es-adaptive table,.es-left,.es-right{width:100%!important}.es-content table,.es-header table,.es-footer table,.es-content,.es-footer,.es-header{width:100%!important;max-width:600px!important}.es-adapt-td{display:block!important;width:100%!important}.adapt-img{width:100%!important;height:auto!important}.es-m-p0{padding:0!important}.es-m-p0r{padding-right:0!important}.es-m-p0l{padding-left:0!important}.es-m-p0t{padding-top:0!important}.es-m-p0b{padding-bottom:0!important}.es-m-p20b{padding-bottom:20px!important}.es-mobile-hidden,.es-hidden{display:none!important}tr.es-desk-hidden,td.es-desk-hidden,table.es-desk-hidden{width:auto!important;overflow:visible!important;float:none!important;max-height:inherit!important;line-height:inherit!important}tr.es-desk-hidden{display:table-row!important}table.es-desk-hidden{display:table!important}td.es-desk-menu-hidden{display:table-cell!important}.es-menu td{width:1%!important}table.es-table-not-adapt,.esd-block-html table{width:auto!important}table.es-social{display:inline-block!important}table.es-social td{display:inline-block!important}.es-m-p5{padding:5px!important}.es-m-p5t{padding-top:5px!important}.es-m-p5b{padding-bottom:5px!important}.es-m-p5r{padding-right:5px!important}.es-m-p5l{padding-left:5px!important}.es-m-p10{padding:10px!important}.es-m-p10t{padding-top:10px!important}.es-m-p10b{padding-bottom:10px!important}.es-m-p10r{padding-right:10px!important}.es-m-p10l{padding-left:10px!important}.es-m-p15{padding:15px!important}.es-m-p15t{padding-top:15px!important}.es-m-p15b{padding-bottom:15px!important}.es-m-p15r{padding-right:15px!important}.es-m-p15l{padding-left:15px!important}.es-m-p20{padding:20px!important}.es-m-p20t{padding-top:20px!important}.es-m-p20r{padding-right:20px!important}.es-m-p20l{padding-left:20px!important}.es-m-p25{padding:25px!important}.es-m-p25t{padding-top:25px!important}.es-m-p25b{padding-bottom:25px!important}.es-m-p25r{padding-right:25px!important}.es-m-p25l{padding-left:25px!important}.es-m-p30{padding:30px!important}.es-m-p30t{padding-top:30px!important}.es-m-p30b{padding-bottom:30px!important}.es-m-p30r{padding-right:30px!important}.es-m-p30l{padding-left:30px!important}.es-m-p35{padding:35px!important}.es-m-p35t{padding-top:35px!important}.es-m-p35b{padding-bottom:35px!important}.es-m-p35r{padding-right:35px!important}.es-m-p35l{padding-left:35px!important}.es-m-p40{padding:40px!important}.es-m-p40t{padding-top:40px!important}.es-m-p40b{padding-bottom:40px!important}.es-m-p40r{padding-right:40px!important}.es-m-p40l{padding-left:40px!important}.es-desk-hidden{display:table-row!important;width:auto!important;overflow:visible!important;max-height:inherit!important}}</style>\r\n</head>\r\n<body style=\"width:100%;font-family:arial,'helvetica neue',helvetica,sans-serif;-webkit-text-size-adjust:100%;-ms-text-size-adjust:100%;padding:0;Margin:0\">\r\n<div class=\"es-wrapper-color\" style=\"background-color:#FAFAFA\"><!--[if gte mso 9]>\r\n<v:background xmlns:v=\"urn:schemas-microsoft-com:vml\" fill=\"t\">\r\n<v:fill type=\"tile\" color=\"#fafafa\"></v:fill>\r\n</v:background>\r\n<![endif]-->\r\n<table class=\"es-wrapper\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" style=\"mso-table-lspace:0;mso-table-rspace:0;border-collapse:collapse;border-spacing:0;padding:0;Margin:0;width:100%;height:100%;background-repeat:repeat;background-position:center top;background-color:#FAFAFA\">\r\n<tr>\r\n<td valign=\"top\" style=\"padding:0;Margin:0\">\r\n<table cellpadding=\"0\" cellspacing=\"0\" class=\"es-content\" align=\"center\" style=\"mso-table-lspace:0;mso-table-rspace:0;border-collapse:collapse;border-spacing:0;table-layout:fixed!important;width:100%\">\r\n<tr>\r\n<td align=\"center\" style=\"padding:0;Margin:0\">\r\n<table bgcolor=\"#ffffff\" class=\"es-content-body\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#FFFFFF;width:600px\">\r\n<tr>\r\n<td align=\"left\" style=\"padding:0;Margin:0;padding-top:15px;padding-left:20px;padding-right:20px\">\r\n<table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n<tr>\r\n<td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;width:560px\">\r\n<table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n<tr>\r\n<td align=\"center\" class=\"es-m-p0r es-m-p0l es-m-txt-c\" style=\"Margin:0;padding-top:15px;padding-bottom:15px;padding-left:40px;padding-right:40px\"><h1 style=\"Margin:0;line-height:55px;mso-line-height-rule:exactly;font-family:arial,'helvetica neue',helvetica,sans-serif;font-size:46px;font-style:normal;font-weight:bold;color:#333333\"><span style=\"color:#4797f2\">Intelli</span><span style=\"color:#eca400\">Test</span></h1></td>\r\n</tr>\r\n<tr>\r\n<td align=\"center\" class=\"es-m-p0r es-m-p0l es-m-txt-c\" style=\"Margin:0;padding-top:15px;padding-bottom:15px;padding-left:40px;padding-right:40px\"><h1 style=\"Margin:0;line-height:55px;mso-line-height-rule:exactly;font-family:arial,'helvetica neue',helvetica,sans-serif;font-size:30px;font-style:normal;font-weight:bold;color:#333333\">Подновяване на паролата</h1></td>\r\n</tr>\r\n<tr>\r\n<td align=\"left\" style=\"padding:0;Margin:0;padding-top:10px\"><p style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:arial,'helvetica neue',helvetica,sans-serif;line-height:21px;color:#333;font-size:14px\">След като кликнеш бутона ще трябва да направиш следното:</p>\r\n<ol>\r\n<li style=\"-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:arial,'helvetica neue',helvetica,sans-serif;line-height:21px;Margin-bottom:15px;margin-left:0;color:#333;font-size:14px\">Въведи нова парола</li>\r\n<li style=\"-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:arial,'helvetica neue',helvetica,sans-serif;line-height:21px;Margin-bottom:15px;margin-left:0;color:#333;font-size:14px\">Повтори паролата</li>\r\n<li style=\"-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:arial,'helvetica neue',helvetica,sans-serif;line-height:21px;Margin-bottom:15px;margin-left:0;color:#333;font-size:14px\">Натисни \"Промяна на паролата\".</li>\r\n</ol></td>\r\n</tr>\r\n</table></td>\r\n</tr>\r\n</table></td>\r\n</tr>\r\n<tr>\r\n<td align=\"left\" style=\"padding:0;Margin:0;padding-bottom:20px;padding-left:20px;padding-right:20px\">\r\n<table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n<tr>\r\n<td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;width:560px\">\r\n<table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:separate;border-spacing:0px;border-radius:5px\" role=\"presentation\">\r\n<tr>\r\n<td align=\"center\" style=\"padding:0;Margin:0;padding-top:10px;padding-bottom:10px\"><span class=\"es-button-border\" style=\"border-style:solid;border-width:0px;display:inline-block;border-radius:6px;width:auto;mso-border-alt:10px\"><a href=\"" +
                link +
                "\" class=\"es-button\" target=\"_blank\" style=\"mso-style-priority:100!important;text-decoration:none;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;color:#fff;font-size:20px;padding:10px 30px 10px 30px;display:inline-block;background:#eca400;border-radius:6px;font-family:arial,'helvetica neue',helvetica,sans-serif;font-weight:normal;font-style:normal;line-height:24px;width:auto;text-align:center;padding-left:30px;padding-right:30px\">Поднови паролата си</a></span></td>\r\n</tr>\r\n<tr>\r\n<td align=\"center\" class=\"es-m-txt-c\" style=\"padding:0;Margin:0;padding-top:10px\"><h3 style=\"Margin:0;line-height:30px;mso-line-height-rule:exactly;font-family:arial,'helvetica neue',helvetica,sans-serif;font-size:20px;font-style:normal;font-weight:bold;color:#333333\">Този линк е валиден само веднъж и ще изтече след 3 часа!</h3></td>\r\n</tr>\r\n</table></td>\r\n</tr>\r\n</table></td>\r\n</tr>\r\n</table></td>\r\n</tr>\r\n</table>\r\n</td>\r\n</tr>\r\n</table>\r\n</div>\r\n</body>\r\n</html>";
        }

        private async Task GiveRole(User user, string roleName)
        {
            var roleExists = await roleManager.RoleExistsAsync(roleName);
            if (roleExists)
            {
                await userManager.AddToRoleAsync(user, roleName);
            }
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
                return RedirectToAction("Login");
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
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            ClearCookies();
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
                IsTeacher = User.IsTeacher()
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

            if (!User.IsTeacher() && !User.IsStudent())
            {
                if (model.IsTeacher)
                {
                    await GiveRole(user, "Teacher");
                    await teacherService.AddTeacher(User.Id());
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
                        IsTeacher = User.IsTeacher()
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
