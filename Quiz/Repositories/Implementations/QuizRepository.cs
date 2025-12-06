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
            .FirstOrDefaultAsync(q => q.Id == id);
    }

    public async Task<IEnumerable<Models.Quiz>> GetPublicQuizzesAsync()
    {
        // Добавим Author и Category для отображения в списке
        return await _context.Quizzes
            .Where(q => q.isPublic)
            .Include(q => q.Author)
            .Include(q => q.Category)
            .ToListAsync();
    }

    public async Task<IEnumerable<Models.Quiz>> GetQuizzesByAuthorAsync(int authorId)
    {
        // Category для отображения
        return await _context.Quizzes
            .Where(q => q.AuthorId == authorId)
            .Include(q => q.Category)
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

    public async Task<Models.Quiz?> GetByIdWithDetailsAsync(int id)
    {
        // Возвращает викторину со всеми деталями: автор, категория, вопросы, варианты и попытки.
        return await _context.Quizzes
            .Include(q => q.Author)
            .Include(q => q.Category)
            .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
            .Include(q => q.Attempts)
            .FirstOrDefaultAsync(q => q.Id == id);
    }

    public async Task DeleteAsync(int id)
    {
        var quiz = await _context.Quizzes.FirstOrDefaultAsync(q => q.Id == id);

        if (quiz != null)
        {
            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();
        }
    }
}
