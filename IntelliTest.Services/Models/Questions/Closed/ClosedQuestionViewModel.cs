namespace IntelliTest.Core.Models.Questions.Closed
{
    public class ClosedQuestionViewModel : QuestionViewModel
    {
        public string[] Answers { get; set; }
        public bool[] AnswerIndexes { get; set; }
        public float MaxScore { get; set; }
    }
}
