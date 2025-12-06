namespace Quiz.DTOs.Quiz;
public class QuizDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? CategoryId { get; set; }
    public bool IsPublic { get; set; }
    public int AuthorId { get; set; }
    public TimeSpan? TimeLimit { get; set; }
    public DateTime CreatedAt { get; set; }
}