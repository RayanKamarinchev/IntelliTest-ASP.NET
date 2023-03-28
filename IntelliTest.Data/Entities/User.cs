using Microsoft.AspNetCore.Identity;

namespace IntelliTest.Data.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IEnumerable<Read> Reads { get; set; }
        public IEnumerable<LessonLike> LessonLikes { get; set; }
        public IEnumerable<TestLike> TestLikes { get; set; }
    }
}
