using System.ComponentModel.DataAnnotations;
using Quiz.Models;

namespace Quiz.DTOs.Question;

public class QuestionCreateDto
{
    [Required]
    public string Text { get; set; }
    [Required]
    public int QuizId { get; set; }
    public QuestionType Type { get; set; }

    [Required]
    [MinLength(2, ErrorMessage = "A question must have at least two options.")]
    public List<OptionRequestDto> Options { get; set; } = new List<OptionRequestDto>();
}

public class OptionRequestDto
{
    [Required]
    public string Text { get; set; }
    public bool IsCorrect { get; set; }
}

public class QuestionUpdateDto
{
    public string? Text { get; set; }
    public QuestionType? Type { get; set; }
    public List<OptionRequestDto>? Options { get; set; }
}