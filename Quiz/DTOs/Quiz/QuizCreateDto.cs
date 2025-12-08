using System.ComponentModel.DataAnnotations;

namespace Quiz.DTOs.Quiz;
public class QuizCreateDto
{
    [Required]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 50 characters.")]
    public string Title { get; set; } = string.Empty;
    
    [StringLength(5000, ErrorMessage = "Description cannot exceed 5000 characters.")]
    public string? Description { get; set; }
    
    [Range(1, int.MaxValue, ErrorMessage = "Invalid Category ID.")]
    public int? CategoryId { get; set; }
    
    [Required]
    public bool IsPublic { get; set; }
    public TimeSpan? TimeLimit { get; set; } = TimeSpan.Zero;
}