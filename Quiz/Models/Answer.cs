namespace Quiz.Models
{
    public class Answer
    {
        public Guid Id { get; set; }
        public string UserAnswer { get; set; }
        public bool IsCorrect { get; set; }
        public Guid AttemptId { get; set; }
        public Guid QuestionId { get; set; }

        public Attempt Attempt { get; set; }
        public Question Question { get; set; }
        
    }
}
