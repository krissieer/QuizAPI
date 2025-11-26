namespace Quiz.Models
{
    public class Attempt
    {
        public Guid Id { get; set; }
        public int Score { get; set; }
        public DateTime TimeSpent { get; set; }
        public DateTime CompletedAt { get; set; }
        public Guid UserId { get; set; }
        public Guid QuizId { get; set; }

        public User User { get; set; }
        public Quiz Quiz { get; set; }
        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }
}
