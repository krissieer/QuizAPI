using Quiz.Models;
using Quiz.Repositories.Implementations;
using Quiz.Repositories.Interfaces;
using Quiz.Services.Interfaces;

namespace Quiz.Services.Implementations;

public class AnswerService : IAnswerService
{
    private readonly IAnswerRepository _answerRepository;
    private readonly IAttemptRepository _attemptRepository;
    private readonly IQuestionRepository _questionRepository;

    public AnswerService(IAnswerRepository answerRepository,
       IAttemptRepository attemptRepository,
       IQuestionRepository questionRepository)
    {
        _answerRepository = answerRepository;
        _attemptRepository = attemptRepository;
        _questionRepository = questionRepository;
    }

    public async Task<Answer?> GetByIdAsync(int id)
    {
        return await _answerRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Answer>> GetAnswersByAttemptAsync(int attemptId)
    {
        return await _answerRepository.GetAnswersByAttemptAsync(attemptId);
    }

    public async Task<Answer> CreateAsync(Answer answer)
    {
        // Проверяем, существует ли попытка
        var attempt = await _attemptRepository.GetByIdAsync(answer.AttemptId);
        if (attempt == null)
            throw new Exception("Attempt not found");

        // Проверяем, существует ли вопрос
        var question = await _questionRepository.GetByIdAsync(answer.QuestionId);
        if (question == null)
            throw new Exception("Question not found");

        // Проверяем, что вопрос относится к тому же Quiz, что и попытка
        if (question.QuizId != attempt.QuizId)
            throw new Exception("Attempt cannot answer a question from a different Quiz");

        await _answerRepository.AddAsync(answer);
        return answer;
    }

    public async Task<bool> UpdateAsync(Answer answer)
    {
        var exists = await _answerRepository.GetByIdAsync(answer.Id);
        if (exists == null) return false;

        await _answerRepository.UpdateAsync(answer);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var answer = await _answerRepository.GetByIdAsync(id);
        if (answer == null) return false;

        await _answerRepository.DeleteAsync(id);
        return true;
    }
}
