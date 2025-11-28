using Microsoft.EntityFrameworkCore;
using Quiz.Models;
using Quiz.Repositories.Interfaces;

namespace Quiz.Repositories.Implementations;

public class QuizRepository : IQuizRepository
{
    private readonly QuizDBContext _context;

    public QuizRepository(QuizDBContext context)
    {
        _context = context;
    }

    public async Task<Models.Quiz?> GetByIdAsync(int id)
    {
        return await _context.Quizzes
            .Include(q => q.Questions)
            .FirstOrDefaultAsync(q => q.Id == id);
    }

    public async Task<IEnumerable<Models.Quiz>> GetPublicQuizzesAsync()
    {
        return await _context.Quizzes
            .Where(q => q.isPublic)
            .ToListAsync();
    }

    public async Task<IEnumerable<Models.Quiz>> GetQuizzesByAuthorAsync(int authorId)
    {
        return await _context.Quizzes
            .Where(q => q.AuthorId == authorId)
            .ToListAsync();
    }

    public async Task AddAsync(Models.Quiz quiz)
    {
        await _context.Quizzes.AddAsync(quiz);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Models.Quiz quiz)
    {
        _context.Quizzes.Update(quiz);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Models.Quiz quiz)
    {
        _context.Quizzes.Remove(quiz);
        await _context.SaveChangesAsync();
    }
}
