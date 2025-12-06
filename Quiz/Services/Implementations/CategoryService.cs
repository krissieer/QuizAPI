using Quiz.Models;
using Quiz.Repositories.Interfaces;
using Quiz.Services.Interfaces;

namespace Quiz.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _categoryRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _categoryRepository.GetAllAsync();
    }

    public async Task<Category> CreateAsync(Category category)
    {
        await _categoryRepository.AddAsync(category);
        return category;
    }

    public async Task<bool> UpdateAsync(Category category)
    {
        var existing = await _categoryRepository.GetByIdAsync(category.Id);
        if (existing == null)
            return false;
        
        existing.Name = category.Name;

        await _categoryRepository.UpdateAsync(existing);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _categoryRepository.GetByIdAsync(id);
        if (existing == null)
            return false;
        if (existing.Quizzes.Any())
            throw new Exception("Cannot delete category with associated quizzes.");

        await _categoryRepository.DeleteAsync(id);
        return true;
    }
}