using Quiz.Models;

namespace Quiz.Repositories.Interfaces;

public interface IQuizRepository
{
    Task<Models.Quiz?> GetByIdAsync(int id);
    Task<Models.Quiz?> GetByIdWithDetailsAsync(int id); // вопросы + опции + автор
    Task<IEnumerable<Models.Quiz>> GetPublicQuizzesAsync();
    Task<IEnumerable<Models.Quiz>> GetQuizzesByAuthorAsync(int authorId);
    Task AddAsync(Models.Quiz quiz);
    Task UpdateAsync(Models.Quiz quiz);
    Task DeleteAsync(int id);
}