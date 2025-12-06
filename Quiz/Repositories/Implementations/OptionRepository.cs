using Microsoft.EntityFrameworkCore;
using Quiz.Models;
using Quiz.Repositories.Interfaces;

namespace Quiz.Repositories.Implementations;

public class OptionRepository : IOptionRepository
{
    private readonly QuizDBContext _context;

    public OptionRepository(QuizDBContext context)
    {
        _context = context;
    }

    public async Task<Option?> GetByIdAsync(int id)
    {
        return await _context.Options.FindAsync(id);
    }

    public async Task<IEnumerable<Option>> GetOptionsByQuestionAsync(int questionId)
    {
        return await _context.Options
            .Where(o => o.QuestionId == questionId)
            .ToListAsync();
    }

    public async Task AddAsync(Option option)
    {
        await _context.Options.AddAsync(option);
        await _context.SaveChangesAsync();
    }

    public async Task AddRangeAsync(IEnumerable<Option> options)
    {
        await _context.Options.AddRangeAsync(options);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Option option)
    {
        _context.Options.Update(option);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var option = await _context.Options.FindAsync(id);
        if (option != null)
        {
            _context.Options.Remove(option);
            await _context.SaveChangesAsync();
        }
    }
}