using Microsoft.EntityFrameworkCore;
using Quiz.Models;
using Quiz.Repositories.Interfaces;

namespace Quiz.Repositories.Implementations;

public class QuestionRepository : IQuestionRepository
{
    private readonly QuizDBContext _context;

    public QuestionRepository(QuizDBContext context)
    {
        _context = context;
    }

    public async Task<Question?> GetByIdAsync(int id)
    {
        return await _context.Questions.FindAsync(id);
    }

    public async Task<Question?> GetByIdWithOptionsAsync(int id)
    {
        return await _context.Questions
            .Include(q => q.Options)
            .FirstOrDefaultAsync(q => q.Id == id);
    }

    public async Task<IEnumerable<Question>> GetQuestionsByQuizAsync(int quizId)
    {
        return await _context.Questions
            .Where(q => q.QuizId == quizId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Question>> GetQuestionsWithOptionsByQuizAsync(int quizId)
    {
        return await _context.Questions
            .Where(q => q.QuizId == quizId)
            .Include(q => q.Options)
            .OrderBy(q => q.Id)
            .ToListAsync();
    }

    public async Task AddAsync(Question question)
    {
        await _context.Questions.AddAsync(question);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Question question)
    {
        _context.Questions.Update(question);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var question = await _context.Questions.FindAsync(id);

        if (question != null)
        {
            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
        }
    }
}
