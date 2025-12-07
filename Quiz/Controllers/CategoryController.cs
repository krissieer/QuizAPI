using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quiz.DTOs.Category;
using Quiz.DTOs.Quiz;
using Quiz.Models;
using Quiz.Services.Implementations;
using Quiz.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Quiz.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    // GET: api/category
    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        var categories = await _categoryService.GetAllAsync() ?? new List<Category>();

        var result = categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name
        });

        return Ok(result);
    }

    // GET: api/category/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);

        if (category == null)
            return NotFound($"Category with ID {id} not found.");

        var result = new CategoryDto
        {
            Id = category.Id,
            Name = category.Name
        };

        return Ok(result);
    }

    // POST: api/category
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var existing = await _categoryService.GetByNameAsync(dto.Name);
            if (existing is not null)
                return Conflict($"Category with name {dto.Name} is alresdy exist");

            var category = new Category
            {
                Name = dto.Name
            };

            var created = await _categoryService.CreateAsync(category);

            return Ok(new CategoryDto
            {
                Id = created.Id,
                Name = created.Name
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
