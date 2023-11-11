namespace IntelliTest.Core.Models.Questions
{
    public class OpenQuestionReviewViewModel : OpenQuestionViewModel
    {
        public string CorrectAnswer { get; set; }
        public decimal Score { get; set; }
        public string Explanation { get; set; }
    }
}
