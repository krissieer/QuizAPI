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

    public async Task AddAsync(Attempt attempt)
    {
        await _context.Attempts.AddAsync(attempt);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Attempt>> GetAttemptsByUserAsync(int userId)
    {
        return await _context.Attempts
            .Where(a => a.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Attempt>> GetAttemptsByQuizAsync(int quizId)
    {
        return await _context.Attempts
            .Where(a => a.QuizId == quizId)
            .ToListAsync();
    }
}
