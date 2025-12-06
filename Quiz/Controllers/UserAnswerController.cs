using Microsoft.AspNetCore.Mvc;
using Quiz.Models;
using Quiz.Services.Interfaces;
using Quiz.DTOs.Answer;
using System.ComponentModel.DataAnnotations;

namespace Quiz.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserAnswerController : ControllerBase
{
    private readonly IUserAnswerService _answerService;
    public UserAnswerController(IUserAnswerService answerService)
    {
        _answerService = answerService;
    }

    private AnswerDto MapToAnswerDto(UserAnswer answer)
    {
        return new AnswerDto
        {
            Id = answer.Id,
            AttemptId = answer.AttemptId,
            QuestionId = answer.QuestionId,
            ChosenOptionId = answer.ChosenOptionId 
        };
    }

    // GET: api/answer/by-attempt/{attemptId}
    [HttpGet("by-attempt/{attemptId}")]
    public async Task<IActionResult> GetByAttempt(int attemptId)
    {
        var answers = await _answerService.GetAnswersByAttemptAsync(attemptId);

        if (!answers.Any())
            return Ok(new List<AnswerDto>());

        var result = answers.Select(MapToAnswerDto);

        return Ok(result);
    }
}