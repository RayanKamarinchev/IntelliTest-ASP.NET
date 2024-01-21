namespace IntelliTest.Infrastructure
{
    public static class Constraints
    {
        public static Guid AdminTeacherId = new Guid("85e17d61-f7c6-4dfc-be35-f4d8ca41f229");

        //TempData
        public static readonly string StudentId = "StudentId";
        public static readonly string TeacherId = "TeacherId";
        public static readonly string Classes = "Classes";
        public static readonly string Message = "message";

        //Messages
        public static readonly string TestEditMsg = "Успешно редактира тест!";
        public static readonly string TestSubmitMsg = "Успешно предаде теста!";
        public static readonly string TestDeleteMsg = "Успешно изтри тест!";

        public static readonly string ClassCreateMsg = "Успешно създаден клас!";
        public static readonly string ClassEditMsg = "Успешно редактиран клас";
        public static readonly string ClassDeleteMsg = "Успешно изтрит клас!";
        public static readonly string EmailSentMsg = "Имейлът е изпратен!";
        public static readonly string PasswordChangedMsg = "Паролата е сменен";

        //Errors
        public static readonly string GradeRangeMsg = "Класът трябва да е между 1 и 12";
        public static readonly string SomethingWrongMsg = "Грешка! Нещо се обърка";

        //Areas
        public static readonly string AdminArea = "Admin";

        //Cache
        public static readonly string TestsCacheKey = "test";
        public static readonly string LessonsCacheKey = "lessons";
        public static readonly string ClassesCacheKey = "classes";
    }
}
