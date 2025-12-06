namespace Quiz.DTOs.Answer;
public class AnswerDto
{
    public int Id { get; set; }
    public int AttemptId { get; set; }
    public int QuestionId { get; set; }
    public int ChosenOptionId { get; set; } //ID выбранной опции
}