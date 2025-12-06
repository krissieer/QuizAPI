using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quiz.DTOs.Question;
using Quiz.Models;
using Quiz.Services.Implementations;
using Quiz.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Quiz.Controllers;

[Route("api/[controller]")]
[ApiController]
public class QuestionController : ControllerBase
{
    private readonly IQuestionService _questionService;

    public QuestionController(IQuestionService questionService)
    {
        _questionService = questionService;
    }

    private QuestionDto MapToQuestionDto(Question q)
    {
        return new QuestionDto
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
        };
    }

    // GET: api/question/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var q = await _questionService.GetByIdAsync(id);
        if (q == null)
            return NotFound($"Question with ID {id} not found.");

        return Ok(MapToQuestionDto(q));
    }

    // GET: api/question/by-quiz/{quizId}
    [HttpGet("by-quiz/{quizId}")]
    public async Task<IActionResult> GetByQuiz(int quizId)
    {
        var questions = await _questionService.GetByQuizAsync(quizId);

        if (!questions.Any())
            return Ok(new List<QuestionDto>());

        var result = questions.Select(MapToQuestionDto);

        return Ok(result);
    }

    // POST: api/question
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] QuestionCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var question = new Question
            {
                Text = dto.Text,
                QuizId = dto.QuizId,
                Type = dto.Type,
            };

            // Разделяем опции из DTO в два списка для сервиса
            var optionTexts = dto.Options.Select(o => o.Text).ToList();
            var isCorrectFlags = dto.Options.Select(o => o.IsCorrect).ToList();

            var created = await _questionService.CreateAsync(question, optionTexts, isCorrectFlags);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToQuestionDto(created));
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // PUT: api/question/{id}
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] QuestionUpdateDto dto)
    {
        var existing = await _questionService.GetByIdAsync(id);
        if (existing == null)
            return NotFound($"Question with ID {id} not found.");

        existing.Text = dto.Text ?? existing.Text;
        existing.Type = dto.Type ?? existing.Type;
        
        List<string>? optionTexts = null;
        List<bool>? isCorrectFlags = null;

        if (dto.Options != null)
        {
            // Разделяем опции из DTO в два списка для сервиса
            optionTexts = dto.Options.Select(o => o.Text).ToList();
            isCorrectFlags = dto.Options.Select(o => o.IsCorrect).ToList();
        }

        var success = await _questionService.UpdateAsync(existing, optionTexts, isCorrectFlags);
        if (!success)
            return StatusCode(500, "Failed to update question.");

        // Получаем обновленный объект с опциями для ответа
        var updated = await _questionService.GetByIdAsync(id);

        return Ok(MapToQuestionDto(updated!));
    }

    // DELETE: api/question/{id}
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _questionService.DeleteAsync(id);
        
        if (!success)
            return NotFound($"Question with ID {id} not found or failed to delete."); 
        return Ok("Deleted");
    }
}