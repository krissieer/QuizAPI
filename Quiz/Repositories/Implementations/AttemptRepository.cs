using Microsoft.EntityFrameworkCore;
using Quiz.Models;
using Quiz.Repositories.Interfaces;

namespace Quiz.Repositories.Implementations;

public class AttemptRepository : IAttemptRepository
{
    private readonly QuizDBContext _context;

    public AttemptRepository(QuizDBContext context)
    {
        _context = context;
    }

    public async Task<Attempt?> GetByIdAsync(int id)
    {
        return await _context.Attempts
            .Include(a => a.Answers)     //  ответы
            .Include(a => a.Quiz)        //  викторина
            .Include(a => a.User)        //  пользователь
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task AddAsync(Attempt attempt)
    {
        await _context.Attempts.AddAsync(attempt);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Attempt attempt)
    {
        _context.Attempts.Update(attempt);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var attempt = await _context.Attempts.FirstOrDefaultAsync(a => a.Id == id);

        if (attempt != null)
        {
            _context.Attempts.Remove(attempt);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Attempt>> GetAttemptsByUserAsync(int userId)
    {
        return await _context.Attempts
            .Where(a => a.UserId == userId)
            .Include(a => a.Answers)
            .Include(a => a.Quiz)
            .ToListAsync();
    }

    public async Task<IEnumerable<Attempt>> GetAttemptsByQuizAsync(int quizId)
    {
        return await _context.Attempts
            .Where(a => a.QuizId == quizId)
            .Include(a => a.Answers)
            .Include(a => a.User)
            .ToListAsync();
    }
}
