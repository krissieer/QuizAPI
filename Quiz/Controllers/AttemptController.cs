using Microsoft.AspNetCore.Mvc;
using Quiz.DTOs.Attempt;
using Quiz.Models;
using Quiz.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

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

    private AttemptDto MapToAttemptDto(Attempt attempt)
    {
         return new AttemptDto
            {
                Id = attempt.Id,
                Score = attempt.Score,
                TimeSpent = attempt.TimeSpent,
                CompletedAt = attempt.CompletedAt,
                UserId = attempt.UserId,
                GuestSessionId = attempt.UserId == null ? attempt.GuestSessionId : null,
                QuizId = attempt.QuizId
            };
    }
    
    private AttemptResultDto MapToAttemptResultDto(Attempt attempt)
    {
         return new AttemptResultDto
            {
                Id = attempt.Id,
                Score = attempt.Score,
                TimeSpent = attempt.TimeSpent,
                CompletedAt = attempt.CompletedAt,
                UserId = attempt.UserId,
                GuestSessionId = attempt.UserId == null ? attempt.GuestSessionId : null,
                QuizId = attempt.QuizId,
                CorrectAnswersCount = attempt.Score //количество правильных ответов
            };
    }

    // POST: api/attempt/quiz-sessions/{quizId}/start
    [HttpPost("quiz-sessions/{quizId}/start")]
    public async Task<IActionResult> StartAttempt(int quizId)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var attempt = await _attemptService.StartAttemptAsync(quizId);
            return Ok(MapToAttemptDto(attempt));
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // POST: api/attempt/quiz-sessions/stop
    [HttpPost("quiz-sessions/stop")] 
    public async Task<IActionResult> FinishAttempt([FromBody] FinishAttemptDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            // Используем новый FinishAttemptDto, содержащий коллекцию SelectedOptionIds
            var attempt = await _attemptService.FinishAttemptAsync(dto.AttemptId, dto.Answers); 
            
            return Ok(MapToAttemptResultDto(attempt));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
             return Conflict(new { error = ex.Message });
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

        return Ok(MapToAttemptDto(attempt));
    }

    // GET: api/attempt/by-user/{userId}
    [HttpGet("by-user/{userId}")]
    public async Task<IActionResult> GetByUser(int userId)
    {
        var attempts = await _attemptService.GetByUserIdAsync(userId);

        if (!attempts.Any())
            return Ok(new List<AttemptDto>());

        var result = attempts.Select(MapToAttemptDto);

        return Ok(result);
    }

    // GET: api/attempt/by-quiz/{quizId}
    [HttpGet("by-quiz/{quizId}")]
    public async Task<IActionResult> GetByQuiz(int quizId)
    {
        var attempts = await _attemptService.GetByQuizIdAsync(quizId);

        if (!attempts.Any())
            return Ok(new List<AttemptDto>());

        var result = attempts.Select(MapToAttemptDto);

        return Ok(result);
    }
}