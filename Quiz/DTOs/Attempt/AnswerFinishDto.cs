using System.ComponentModel.DataAnnotations;

namespace Quiz.DTOs.Attempt;

public class AnswerFinishDto
{
    public int QuestionId { get; set; }
    public List<int> SelectedOptionIds { get; set; } = new();
}