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

    public async Task AddAsync(Answer answer)
    {
        await _context.Answers.AddAsync(answer);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Answer>> GetAnswersByAttemptAsync(int attemptId)
    {
        return await _context.Answers
            .Where(q => q.AttemptId == attemptId)
            .ToListAsync();
    }
}
