using Quiz.Models;
using Quiz.DTOs.Attempt;

namespace Quiz.Services.Interfaces;

public interface IAttemptService
{
    Task<Attempt?> GetByIdAsync(int id);
    Task<IEnumerable<Attempt>> GetByUserIdAsync(int userId);
    Task<IEnumerable<Attempt>> GetByQuizIdAsync(int quizId);
    Task<Attempt> StartAttemptAsync(int quizId);
    Task<Attempt> FinishAttemptAsync(int attemptId, IEnumerable<AnswerFinishDto> answers);
    Task<bool> DeleteAsync(int id);
}
