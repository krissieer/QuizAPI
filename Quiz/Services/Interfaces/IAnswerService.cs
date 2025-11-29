using Quiz.Models;
using System.Threading.Tasks;

namespace Quiz.Services.Interfaces;

public interface IAnswerService
{
    Task<Answer?> GetByIdAsync(int id);
    Task<IEnumerable<Answer>> GetAnswersByAttemptAsync(int attemptId);
    Task<Answer> CreateAsync(Answer answer);
    Task<bool> UpdateAsync(Answer answer);
    Task<bool> DeleteAsync(int id);
}
