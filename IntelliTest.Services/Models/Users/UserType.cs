using System.ComponentModel.DataAnnotations;

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
