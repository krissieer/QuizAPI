using Microsoft.AspNetCore.Mvc;
using Quiz.Models;
using Quiz.Services.Interfaces;
using Quiz.DTOs.Attempt;
using System.ComponentModel.DataAnnotations;

namespace Quiz.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AttemptController : ControllerBase
{
    private readonly IAttemptService _attemptService;

    public AttemptController(IAttemptService attemptService)
    {
        _attemptService = attemptService;
    }

    // POST: api/attempt/quiz-sessions/{quizId}/start
    [HttpPost("quiz-sessions/{quizId}/start")]
    public async Task<IActionResult> StartAttempt(int quizId, [FromBody] StartAttemptDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var attempt = await _attemptService.StartAttemptAsync(dto.UserId, quizId);

            return Ok(new AttemptDto
            {
                Id = attempt.Id,
                Score = attempt.Score,
                TimeSpent = attempt.TimeSpent,
                CompletedAt = attempt.CompletedAt,
                UserId = attempt.UserId,
                QuizId = attempt.QuizId
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // POST: api/attempt/quiz-sessions/{quizId}/stop
    // Внимание: quizId не используется — завершаем по attemptId
    [HttpPost("quiz-sessions/stop")]
    public async Task<IActionResult> FinishAttempt([FromBody] FinishAttemptDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var attempt = await _attemptService.FinishAttemptAsync(dto.AttemptId, dto.Answers);

            return Ok(new AttemptResultDto
            {
                Id = attempt.Id,
                Score = attempt.Score,
                TimeSpent = attempt.TimeSpent,
                CompletedAt = attempt.CompletedAt,
                UserId = attempt.UserId,
                QuizId = attempt.QuizId,
                CorrectAnswersCount = attempt.Score
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // GET: api/attempt/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var attempt = await _attemptService.GetByIdAsync(id);
        if (attempt == null)
            return NotFound($"Attempt with ID {id} not found.");

        return Ok(new AttemptDto
        {
            Id = attempt.Id,
            Score = attempt.Score,
            TimeSpent = attempt.TimeSpent,
            CompletedAt = attempt.CompletedAt,
            UserId = attempt.UserId,
            QuizId = attempt.QuizId
        });
    }

    // GET: api/attempt/by-user/{userId}
    [HttpGet("by-user/{userId}")]
    public async Task<IActionResult> GetByUser(int userId)
    {
        var attempts = await _attemptService.GetByUserIdAsync(userId);

        if (!attempts.Any())
            return Ok(new List<AttemptDto>());

        var result = attempts.Select(a => new AttemptDto
        {
            Id = a.Id,
            Score = a.Score,
            TimeSpent = a.TimeSpent,
            CompletedAt = a.CompletedAt,
            UserId = a.UserId,
            QuizId = a.QuizId
        });

        return Ok(result);
    }

    // GET: api/attempt/by-quiz/{quizId}
    [HttpGet("by-quiz/{quizId}")]
    public async Task<IActionResult> GetByQuiz(int quizId)
    {
        var attempts = await _attemptService.GetByQuizIdAsync(quizId);

        if (!attempts.Any())
            return Ok(new List<AttemptDto>());

        var result = attempts.Select(a => new AttemptDto
        {
            Id = a.Id,
            Score = a.Score,
            TimeSpent = a.TimeSpent,
            CompletedAt = a.CompletedAt,
            UserId = a.UserId,
            QuizId = a.QuizId
        });

        return Ok(result);
    }
}

// DTO
public class StartAttemptDto
{
    [Required]
    public int UserId { get; set; }
}

public class AttemptResultDto : AttemptDto
{
    public int CorrectAnswersCount { get; set; }
}
