using Quiz.Models;

namespace Quiz.Repositories.Interfaces;

public interface IAttemptRepository
{
    Task AddAsync(Attempt attempt);
    Task<IEnumerable<Attempt>> GetAttemptsByUserAsync(int userId);
    Task<IEnumerable<Attempt>> GetAttemptsByQuizAsync(int quizId);
}
