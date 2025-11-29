using Quiz.Repositories.Interfaces;
using Quiz.Services.Interfaces;

namespace Quiz.Services.Implementations;

public class QuizService : IQuizService
{
    private readonly IQuizRepository _quizRepository;
    private readonly IUserRepository _userRepository;

    public QuizService(IQuizRepository quizRepository, IUserRepository userRepository)
    {
        _quizRepository = quizRepository;
        _userRepository = userRepository;
    }

    public async Task<Models.Quiz?> GetByIdAsync(int id)
    {
        return await _quizRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Models.Quiz>> GetAllPublicAsync()
    {
        return await _quizRepository.GetPublicQuizzesAsync();
    }

    public async Task<IEnumerable<Models.Quiz>> GetByAuthorAsync(int authorId)
    {
        return await _quizRepository.GetQuizzesByAuthorAsync(authorId);
    }

    public async Task<Models.Quiz> CreateAsync(Models.Quiz quiz)
    {
        var author = await _userRepository.GetByIdAsync(quiz.AuthorId);
        if (author == null)
            throw new Exception("Author not found");

        quiz.CreatedAt = DateTime.UtcNow;

        await _quizRepository.AddAsync(quiz);
        return quiz;
    }

    public async Task<bool> UpdateAsync(Models.Quiz quiz)
    {
        var existing = await _quizRepository.GetByIdAsync(quiz.Id);
        if (existing == null)
            return false;

        existing.Title = quiz.Title;
        existing.Description = quiz.Description;
        existing.Category = quiz.Category;
        existing.Language = quiz.Language;
        existing.isPublic = quiz.isPublic;
        existing.TimeLimit = quiz.TimeLimit;

        await _quizRepository.UpdateAsync(existing);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _quizRepository.GetByIdAsync(id);
        if (existing == null)
            return false;

        await _quizRepository.DeleteAsync(existing);
        return true;
    }
    }
