using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quiz.DTOs.Attempt;
using Quiz.DTOs.Question;
using Quiz.DTOs.Quiz;
using Quiz.Models;
using Quiz.Services.Implementations;
using Quiz.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;


namespace Quiz.Controllers;

[Route("api/[controller]")]
[ApiController]
public class QuizController : ControllerBase
{
    private readonly IQuizService _quizService;
    private readonly IQuestionService _questionService;
    private readonly IAttemptService _attemptService;

    public QuizController(IQuizService quizService, IQuestionService questionService, IAttemptService attemptService)
    {
        _quizService = quizService;
        _questionService = questionService;
        _attemptService = attemptService;
    }

    // GET: api/quiz
    [HttpGet]
    public async Task<IActionResult> GetAllPublic()
    {
        var quizzes = await _quizService.GetAllPublicAsync() ?? new List<Models.Quiz>();

        if (!quizzes.Any())
            return Ok(new List<QuizDto>());

        var result = quizzes.Select(q => new QuizDto
        {
            Id = q.Id,
            Title = q.Title,
            Description = q.Description,
            CategoryId = q.CategoryId,
            IsPublic = q.isPublic,
            AuthorId = q.AuthorId,
            TimeLimit = q.TimeLimit,
            CreatedAt = q.CreatedAt
        });

        return Ok(result);
    }

    // GET: api/quiz/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var quiz = await _quizService.GetByIdAsync(id);
        if (quiz == null)
            return NotFound($"Quiz with ID {id} not found.");

        var result = new QuizDto
        {
            Id = quiz.Id,
            Title = quiz.Title,
            Description = quiz.Description,
            CategoryId = quiz.CategoryId,
            IsPublic = quiz.isPublic,
            AuthorId = quiz.AuthorId,
            TimeLimit = quiz.TimeLimit,
            CreatedAt = quiz.CreatedAt
        };

        return Ok(result);
    }

    // GET: api/quiz/{quizId}/questions
    [HttpGet("{quizId}/questions")]
    public async Task<IActionResult> GetQuestions(int quizId)
    {
        var questions = await _questionService.GetByQuizAsync(quizId);

        if (!questions.Any())
            return Ok(new List<QuestionDto>());

        var result = questions.Select(q => new QuestionDto
        {
            Id = q.Id,
            Text = q.Text,
            Type = q.Type,
            QuizId = q.QuizId,
            Options = q.Options.Select(o => new OptionDto
            {
                Id = o.Id,
                Text = o.Text,
            }).ToList()
        });

        return Ok(result);
    }

    // GET: api/quiz/{quizId}/attempts
    [HttpGet("{quizId}/attempts")]
    public async Task<IActionResult> GetAttepmts(int quizId)
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
            GuestSessionId = a.UserId == null ? a.GuestSessionId : null,
            QuizId = a.QuizId
        });
        return Ok(result);
    }

    // POST: api/quiz
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] QuizCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(user))
            return Unauthorized("User not authenticated.");
        int authorizedUserId = int.Parse(user);

        try
        {
            var quiz = new Models.Quiz
            {
                Title = dto.Title,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                isPublic = dto.IsPublic,
                AuthorId = authorizedUserId,
                TimeLimit = dto.TimeLimit,
                CreatedAt = DateTime.Now
            };

            var created = await _quizService.CreateAsync(quiz);

            return Ok(new QuizDto
            {
                Id = created.Id,
                Title = created.Title,
                Description = created.Description,
                CategoryId = created.CategoryId,
                IsPublic = created.isPublic,
                AuthorId = created.AuthorId,
                TimeLimit = created.TimeLimit,
                CreatedAt = created.CreatedAt
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // PUT: api/quiz/{id}
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] QuizUpdateDto dto)
    {
        var existing = await _quizService.GetByIdAsync(id);
        if (existing == null)
            return NotFound($"Quiz with ID {id} not found.");

        var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        int authorizedUserId = int.Parse(user);
        if (existing.AuthorId != authorizedUserId)
            return Conflict("You have no access to edit this quiz");

        existing.Title = dto.Title ?? existing.Title;
        existing.Description = dto.Description ?? existing.Description;
        existing.CategoryId = dto.CategoryId ?? existing.CategoryId;
        existing.isPublic = dto.IsPublic ?? existing.isPublic;
        existing.TimeLimit = dto.TimeLimit ?? existing.TimeLimit;

        var success = await _quizService.UpdateAsync(existing);
        if (!success)
            return StatusCode(500, "Failed to update quiz due to server error.");

        return Ok("Updated");
    }

    // DELETE: api/quiz/{id}
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var quiz = await _quizService.GetByIdAsync(id);
        if (quiz == null)
            return NotFound($"Quiz with ID {id} not found.");

        var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        int authorizedUserId = int.Parse(user);
        if (quiz.AuthorId != authorizedUserId)
            return Conflict("You have no access to edit this quiz");

        var success = await _quizService.DeleteAsync(id);
        if (!success)
            return StatusCode(500, "Failed to delete quiz due to server error.");
        return Ok("Deleted");
    }

}
