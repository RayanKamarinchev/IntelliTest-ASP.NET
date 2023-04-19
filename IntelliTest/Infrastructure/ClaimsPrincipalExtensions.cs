using System.Security.Claims;

namespace IntelliTest.Infrastructure
{
    public static class ClaimsPrincipalExtensions
    {
        public static string Id(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public static bool IsTeacher(this ClaimsPrincipal user)
        {
            return user.IsInRole("Teacher");
        }
        public static bool IsStudent(this ClaimsPrincipal user)
        {
            return user.IsInRole("Student");
        }
    }
}
