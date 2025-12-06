using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quiz.DTOs.Quiz;
using Quiz.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;


namespace Quiz.Controllers;

[Route("api/[controller]")]
[ApiController]
public class QuizController : ControllerBase
{
    private readonly IQuizService _quizService;

    public QuizController(IQuizService quizService)
    {
        _quizService = quizService;
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

    // GET: api/quiz/by-author/{authorId}
    [HttpGet("by-author/{authorId}")]
    public async Task<IActionResult> GetByAuthor(int authorId)
    {
        var quizzes = await _quizService.GetByAuthorAsync(authorId);

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
        var success = await _quizService.DeleteAsync(id);
        if (!success)
            return StatusCode(500, "Failed to delete quiz due to server error.");
        return Ok("Deleted");
    }

}
