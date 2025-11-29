namespace Quiz.Services.Interfaces;

public interface IQuizService
{
    Task<Models.Quiz?> GetByIdAsync(int id);
    Task<IEnumerable<Models.Quiz>> GetAllPublicAsync();
    Task<IEnumerable<Models.Quiz>> GetByAuthorAsync(int authorId);
    Task<Models.Quiz> CreateAsync(Models.Quiz quiz);
    Task<bool> UpdateAsync(Models.Quiz quiz);
    Task<bool> DeleteAsync(int id);
}
