using System.ComponentModel.DataAnnotations;

namespace Quiz.DTOs.Quiz;
public class QuizCreateDto
{
    [Required]
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? CategoryId { get; set; }
    [Required]
    public bool IsPublic { get; set; }
    public TimeSpan? TimeLimit { get; set; } = TimeSpan.Zero;
}