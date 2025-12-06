using System.ComponentModel.DataAnnotations;
using Quiz.Models;

namespace Quiz.DTOs.Question;

public class OptionDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
}

public class OptionAdminDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}

