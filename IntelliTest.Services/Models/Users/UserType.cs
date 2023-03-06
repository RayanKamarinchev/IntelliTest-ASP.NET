using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Core.Models.Users
{
    public class UserType
    {
        public bool IsStudent { get; set; }
        [Required]
        public int Grade { get; set; }
        [Required]
        public string School { get; set; }
    }
}
