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
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Attempt?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.Attempts
            .Include(a => a.User)
            .Include(a => a.Quiz)
                .ThenInclude(q => q.Questions!)
                    .ThenInclude(q => q.Options)
            .Include(a => a.UserAnswers!)
                .ThenInclude(ua => ua.ChosenOption)
            .Include(a => a.UserAnswers!)
                .ThenInclude(ua => ua.Question)
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
        var attempt = await _context.Attempts.FindAsync(id);
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
            .Include(a => a.Quiz)
            .OrderByDescending(a => a.CompletedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Attempt>> GetAttemptsByQuizAsync(int quizId)
    {
        return await _context.Attempts
            .Where(a => a.QuizId == quizId)
            .Include(a => a.User)
            .OrderByDescending(a => a.Score)
            .ThenBy(a => a.TimeSpent)
            .ToListAsync();
    }

    // Получить попытки конкретного пользователя в конкретной викторине
    public async Task<IEnumerable<Attempt>> GetAttemptsByUserIdAndQuizIdAsync(int userId, int quizId)
    {
        return await _context.Attempts
            .Where(a => a.UserId == userId && a.QuizId == quizId)
            .Include(a => a.Quiz)
            .OrderByDescending(a => a.CompletedAt)
            .ToListAsync();
    }

    // Получить попытки конкретного гостя в конкретной викторине
    public async Task<IEnumerable<Attempt>> GetAttemptsByGuestIdAndQuizIdAsync(string guestSessionId, int quizId)
    {
        return await _context.Attempts
            .Where(a => a.GuestSessionId == guestSessionId && a.QuizId == quizId)
            .Include(a => a.Quiz)
            .OrderByDescending(a => a.CompletedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Attempt>> GetLeaderboardAsync(int? quizId = null)
    {
        var query = _context.Attempts
            .Include(a => a.User)
            .Include(a => a.Quiz)
            .AsQueryable();

        if (quizId.HasValue)
        {
            query = query.Where(a => a.QuizId == quizId.Value);
        }

        return await query
            .OrderByDescending(a => a.Score)
            .ThenBy(a => a.TimeSpent)
            .Take(100) // топ-100, можно параметризовать
            .ToListAsync();
    }
}