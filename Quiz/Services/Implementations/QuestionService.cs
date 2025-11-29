using Quiz.Models;
using Quiz.Repositories.Interfaces;
using Quiz.Services.Interfaces;

namespace Quiz.Services.Implementations;

public class QuestionService : IQuestionService
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IQuizRepository _quizRepository;

    public QuestionService(IQuestionRepository questionRepository, IQuizRepository quizRepository)
    {
        _questionRepository = questionRepository;
        _quizRepository = quizRepository;
    }

    public async Task<Question?> GetByIdAsync(int id)
    {
        return await _questionRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Question>> GetByQuizAsync(int quizId)
    {
        return await _questionRepository.GetQuestionsByQuizAsync(quizId);
    }

    public async Task<Question> CreateAsync(Question question)
    {
        // Проверяем, существует ли викторина
        var quiz = await _quizRepository.GetByIdAsync(question.QuizId);
        if (quiz == null)
            throw new Exception("Quiz not found");

        // Для безопасности: корректный формат списков
        question.Options ??= new List<string>();
        question.CorrectAnswer ??= new List<string>();

        await _questionRepository.AddAsync(question);
        return question;
    }

    public async Task<bool> UpdateAsync(Question question)
    {
        var existing = await _questionRepository.GetByIdAsync(question.Id);
        if (existing == null)
            return false;

        existing.Text = question.Text;
        existing.Type = question.Type;
        existing.Options = question.Options ?? new List<string>();
        existing.CorrectAnswer = question.CorrectAnswer ?? new List<string>();

        await _questionRepository.UpdateAsync(existing);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _questionRepository.GetByIdAsync(id);
        if (existing == null)
            return false;

        await _questionRepository.DeleteAsync(existing);
        return true;
    }
}
