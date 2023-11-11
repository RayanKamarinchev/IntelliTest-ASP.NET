using Microsoft.AspNetCore.Http;

namespace IntelliTest.Core.Models.Users
{
    public class EditUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? Password { get; set; }
        public bool IsTeacher { get; set; }
        public UserPanel UserPanel { get; set; }
        public IFormFile? Image { get; set; }
        public string ImageUrl { get; set; } = "";
        public string School { get; set; } = "";
    }
}
