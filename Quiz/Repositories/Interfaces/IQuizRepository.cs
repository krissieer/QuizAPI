using Quiz.Models;

namespace Quiz.Repositories.Interfaces;

public interface IQuizRepository
{
    Task<Models.Quiz?> GetByIdAsync(int id);
    Task<IEnumerable<Models.Quiz>> GetPublicQuizzesAsync();
    Task<IEnumerable<Models.Quiz>> GetQuizzesByAuthorAsync(int authorId);
    Task AddAsync(Models.Quiz quiz);
    Task UpdateAsync(Models.Quiz quiz);
    Task DeleteAsync(Models.Quiz quiz);
}
