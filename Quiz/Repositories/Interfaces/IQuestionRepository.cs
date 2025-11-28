using Quiz.Models;

namespace Quiz.Repositories.Interfaces;

public interface IQuestionRepository
{
    Task<Question?> GetByIdAsync(int id);
    Task<IEnumerable<Question>> GetQuestionsByQuizAsync(int quizId);
    Task AddAsync(Question question);
    Task UpdateAsync(Question question);
    Task DeleteAsync(Question question);
}
