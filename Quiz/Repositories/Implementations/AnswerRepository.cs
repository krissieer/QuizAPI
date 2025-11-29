using Microsoft.EntityFrameworkCore;
using Quiz.Models;
using Quiz.Repositories.Interfaces;

namespace Quiz.Repositories.Implementations;

public class AnswerRepository : IAnswerRepository
{
    private readonly QuizDBContext _context;

    public AnswerRepository(QuizDBContext context)
    {
        _context = context;
    }

    public async Task<Answer?> GetByIdAsync(int id)
    {
        return await _context.Answers
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task AddAsync(Answer answer)
    {
        await _context.Answers.AddAsync(answer);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Answer answer)
    {
        _context.Answers.Update(answer);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var answer = await _context.Answers.FirstOrDefaultAsync(a => a.Id == id);

        if (answer != null)
        {
            _context.Answers.Remove(answer);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Answer>> GetAnswersByAttemptAsync(int attemptId)
    {
        return await _context.Answers
            .Where(a => a.AttemptId == attemptId)
            .ToListAsync();
    }
}
