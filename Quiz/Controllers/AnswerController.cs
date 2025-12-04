using Microsoft.AspNetCore.Mvc;
using Quiz.Models;
using Quiz.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Quiz.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AnswerController : ControllerBase
{
    private readonly IAnswerService _answerService;
    public AnswerController(IAnswerService answerService)
    {
        _answerService = answerService;
    }

    // POST: api/answer/quiz-sessions/{questionId}/answer
    [HttpPost("quiz-sessions/{questionId}/answer")]
    public async Task<IActionResult> SaveAnswer(int questionId, [FromBody] AnswerCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var answer = new Answer
            {
                QuestionId = questionId,
                AttemptId = dto.AttemptId,
                UserAnswer = dto.UserAnswer ?? string.Empty
                //IsCorrect будет вычислен в сервисе автоматически!
            };

            var created = await _answerService.CreateAsync(answer);

            return Ok(new AnswerDto
            {
                Id = created.Id,
                UserAnswer = created.UserAnswer,
                IsCorrect = created.IsCorrect,
                AttemptId = created.AttemptId,
                QuestionId = created.QuestionId
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // PATCH: api/answer/quiz-sessions/{questionId}/answer
    [HttpPatch("quiz-sessions/{questionId}/answer")]
    public async Task<IActionResult> UpdateAnswer(int questionId, [FromBody] AnswerUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Находим существующий ответ
        var answers = await _answerService.GetAnswersByAttemptAsync(dto.AttemptId);
        var existing = answers.FirstOrDefault(a => a.QuestionId == questionId);

        if (existing == null)
            return NotFound($"Answer for question {questionId} in attempt {dto.AttemptId} not found.");

        existing.UserAnswer = dto.UserAnswer ?? existing.UserAnswer;

        var success = await _answerService.UpdateAsync(existing);
        if (!success)
            return StatusCode(500, "Failed to update answer.");

        // Перезагружаем, чтобы получить актуальный IsCorrect
        var updated = (await _answerService.GetAnswersByAttemptAsync(dto.AttemptId))
            .First(a => a.QuestionId == questionId);

        return Ok(new AnswerDto
        {
            Id = updated.Id,
            UserAnswer = updated.UserAnswer,
            IsCorrect = updated.IsCorrect,
            AttemptId = updated.AttemptId,
            QuestionId = updated.QuestionId
        });
    }

    // GET: api/answer/by-attempt/{attemptId}
    [HttpGet("by-attempt/{attemptId}")]
    public async Task<IActionResult> GetByAttempt(int attemptId)
    {
        var answers = await _answerService.GetAnswersByAttemptAsync(attemptId);

        if (!answers.Any())
            return Ok(new List<AnswerDto>());

        var result = answers.Select(a => new AnswerDto
        {
            Id = a.Id,
            UserAnswer = a.UserAnswer,
            IsCorrect = a.IsCorrect,
            AttemptId = a.AttemptId,
            QuestionId = a.QuestionId
        });

        return Ok(result);
    }
}

// DTO
public class AnswerDto
{
    public int Id { get; set; }
    public string UserAnswer { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int AttemptId { get; set; }
    public int QuestionId { get; set; }
}

public class AnswerCreateDto
{
    [Required] public int AttemptId { get; set; }
    [Required] public string? UserAnswer { get; set; }
}

public class AnswerUpdateDto
{
    [Required] public int AttemptId { get; set; }
    public string? UserAnswer { get; set; }
}