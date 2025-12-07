using System.ComponentModel.DataAnnotations;

namespace Quiz.DTOs.Category;

public class CreateCategoryDto
{
    [Required]
    public string Name { get; set; }
}
