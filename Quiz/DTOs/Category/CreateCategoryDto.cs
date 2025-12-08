using System.ComponentModel.DataAnnotations;

namespace Quiz.DTOs.Category;

public class CreateCategoryDto
{
    [Required]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Category name must be between 2 and 50 characters.")] // Добавлена длина
    public string Name { get; set; }
}
