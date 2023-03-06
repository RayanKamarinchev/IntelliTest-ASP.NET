using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;

namespace IntelliTest.Core.Models.Users
{
    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public bool RememberMe { get; set; }
    }
}
