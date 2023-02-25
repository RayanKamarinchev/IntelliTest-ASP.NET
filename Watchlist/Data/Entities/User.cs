using Microsoft.AspNetCore.Identity;

namespace IntelliTest.Data.Entities
{
    public class User : IdentityUser
    {
        public List<Test> Tests { get; set; } = new List<Test>();
        public bool IsDeleted { get; set; }
    }
}
