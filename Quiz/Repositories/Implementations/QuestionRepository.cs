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
        return await _context.Questions
            .FirstOrDefaultAsync(q => q.Id == id);
    }

    public async Task<IEnumerable<Question>> GetQuestionsByQuizAsync(int quizId)
    {
        return await _context.Questions
           .Where(q => q.QuizId == quizId)
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

    public async Task DeleteAsync(Question question)
    {
        _context.Questions.Remove(question);
        await _context.SaveChangesAsync();
    }
}
