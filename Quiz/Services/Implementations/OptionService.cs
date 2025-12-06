using Quiz.Models;
using Quiz.Repositories.Interfaces;
using Quiz.Services.Interfaces;

namespace Quiz.Services.Implementations;

public class OptionService : IOptionService
{
    private readonly IOptionRepository _optionRepository;
    private readonly IQuestionRepository _questionRepository;

    public OptionService(IOptionRepository optionRepository, IQuestionRepository questionRepository)
    {
        _optionRepository = optionRepository;
        _questionRepository = questionRepository;
    }

    public async Task<Option?> GetByIdAsync(int id)
    {
        return await _optionRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Option>> GetByQuestionAsync(int questionId)
    {
        return await _optionRepository.GetOptionsByQuestionAsync(questionId);
    }

    public async Task<Option> CreateAsync(Option option)
    {
        var question = await _questionRepository.GetByIdAsync(option.QuestionId);
        if (question == null)
            throw new Exception("Question not found");

        await _optionRepository.AddAsync(option);
        return option;
    }

    public async Task AddRangeAsync(IEnumerable<Option> options)
    {
        // Здесь можно добавить проверку, что все опции принадлежат существующему Question
        await _optionRepository.AddRangeAsync(options);
    }

    public async Task<bool> UpdateAsync(Option option)
    {
        var existing = await _optionRepository.GetByIdAsync(option.Id);
        if (existing == null)
            return false;

        // Обновляем только разрешенные поля
        existing.Text = option.Text;
        existing.IsCorrect = option.IsCorrect;
        
        // Проверка: нельзя менять привязку к Question
        if (existing.QuestionId != option.QuestionId)
            throw new Exception("Cannot change option's question association.");

        await _optionRepository.UpdateAsync(existing);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _optionRepository.GetByIdAsync(id);
        if (existing == null)
            return false;

        await _optionRepository.DeleteAsync(id);
        return true;
    }
}