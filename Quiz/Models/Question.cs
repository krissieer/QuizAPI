namespace Quiz.Models
{
    public class Question
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public Guid QuestionTypeId { get; set; }
        public List<string>? Options { get; set; }
        public List<string> CorrectAnswer { get; set; }
        public Guid QuizId { get; set; }

        public QuestionType Type { get; set; }
        public Quiz Quiz { get; set; }
        public ICollection<Answer> Answers { get; set; } = new List<Answer>();

    }
}
