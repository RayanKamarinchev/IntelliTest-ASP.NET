using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Core.Models.Users
{
    public class ResetPassword
    {

        public string Token { get; set; }
        [StringLength(20, MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string Email { get; set; }
        [Compare(nameof(Password))]
        public string RepeatPassword { get; set; }
    }
}
