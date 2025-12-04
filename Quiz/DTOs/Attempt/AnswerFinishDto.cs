using System.ComponentModel.DataAnnotations;

namespace Quiz.DTOs.Attempt;

public class AnswerFinishDto
{
    [Required]
    public int QuestionId { get; set; }

    [Required]
    public string UserAnswer { get; set; } = string.Empty;
}