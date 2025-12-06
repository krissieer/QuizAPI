using System.ComponentModel.DataAnnotations;

namespace Quiz.DTOs.Attempt;

public class FinishAttemptDto
{
    [Required]
    public int AttemptId { get; set; }

    [Required]
    public IEnumerable<AnswerFinishDto> Answers { get; set; } = new List<AnswerFinishDto>();
}