namespace Quiz.Models
{
    public class Quiz
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string Language { get; set; }
        public bool isPublic { get; set; }
        public DateTime TimeLimit  { get; set; }
        public Guid AuthorId { get; set; }
        public DateTime CreatedAt { get; set; }

        public User Author { get; set; }
        public ICollection<Attempt> Attempts { get; set; } = new List<Attempt>();
        public ICollection<Question> Questions { get; set; } = new List<Question>();

    }
}
