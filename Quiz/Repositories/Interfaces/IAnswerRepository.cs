using Quiz.Models;

namespace Quiz.Repositories.Interfaces;

public interface IAnswerRepository
{
    Task<Answer?> GetByIdAsync(int id);
    Task AddAsync(Answer answer);
    Task UpdateAsync(Answer answer);
    Task DeleteAsync(int id);
    Task<IEnumerable<Answer>> GetAnswersByAttemptAsync(int attemptId);
}
