using Quiz.Models;

namespace Quiz.Repositories.Interfaces;

public interface IAnswerRepository
{
    Task AddAsync(Answer answer);
    Task<IEnumerable<Answer>> GetAnswersByAttemptAsync(int attemptId);
}
