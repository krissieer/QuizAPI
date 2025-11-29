using Quiz.Models;

namespace Quiz.Repositories.Interfaces;

public interface IAnswerRepository
{
    Task<Answer?> GetByIdAsync(int id);
    Task AddAsync(Answer answer);
    Task<IEnumerable<Answer>> GetAnswersByAttemptAsync(int attemptId);
    Task UpdateAsync(Answer answer);
    Task DeleteAsync(int id);
}
