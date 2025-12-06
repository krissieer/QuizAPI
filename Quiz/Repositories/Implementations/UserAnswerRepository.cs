using Microsoft.EntityFrameworkCore;
using Quiz.Models;
using Quiz.Repositories.Interfaces;

namespace Quiz.Repositories.Implementations;

public class UserAnswerRepository : IUserAnswerRepository
{
    private readonly QuizDBContext _context;

    public UserAnswerRepository(QuizDBContext context)
    {
        _context = context;
    }

    public async Task<UserAnswer?> GetByIdAsync(int id)
    {
        return await _context.UserAnswers
            .Include(ua => ua.Attempt) 
            .Include(ua => ua.Question) 
            .Include(ua => ua.ChosenOption) 
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task AddAsync(UserAnswer answer)
    {
        await _context.UserAnswers.AddAsync(answer);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(UserAnswer answer)
    {
        _context.UserAnswers.Update(answer);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var answer = await _context.UserAnswers.FirstOrDefaultAsync(a => a.Id == id);

        if (answer != null)
        {
            _context.UserAnswers.Remove(answer);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<UserAnswer>> GetAnswersByAttemptAsync(int attemptId)
    {
        return await _context.UserAnswers
            .Where(a => a.AttemptId == attemptId)
            .Include(ua => ua.Question)
            .Include(ua => ua.ChosenOption)
            .ToListAsync();
    }
}
