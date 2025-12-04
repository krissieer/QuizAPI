using Quiz.Models;
using Quiz.Repositories.Interfaces;
using Quiz.Services.Interfaces;
using Quiz.DTOs.Attempt;

namespace Quiz.Services.Implementations;

public class AttemptService : IAttemptService
{
    private readonly IAttemptRepository _attemptRepository;
    private readonly IUserRepository _userRepository;
    private readonly IQuizRepository _quizRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly IAnswerRepository _answerRepository;

    public AttemptService(
        IAttemptRepository attemptRepository,
        IUserRepository userRepository,
        IQuizRepository quizRepository,
        IQuestionRepository questionRepository,
        IAnswerRepository answerRepository)
    {
        _attemptRepository = attemptRepository;
        _userRepository = userRepository;
        _quizRepository = quizRepository;
        _questionRepository = questionRepository;
        _answerRepository = answerRepository;
    }

    public async Task<Attempt?> GetByIdAsync(int id)
    {
        return await _attemptRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Attempt>> GetByUserIdAsync(int userId)
    {
        return await _attemptRepository.GetAttemptsByUserAsync(userId);
    }

    public async Task<IEnumerable<Attempt>> GetByQuizIdAsync(int quizId)
    {
        return await _attemptRepository.GetAttemptsByQuizAsync(quizId);
    }

    public async Task<Attempt> StartAttemptAsync(int userId, int quizId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new Exception("User not found");

        var quiz = await _quizRepository.GetByIdAsync(quizId);
        if (quiz == null)
            throw new Exception("Quiz not found");

        var attempt = new Attempt
        {
            UserId = userId,
            QuizId = quizId,
            CompletedAt = DateTime.MinValue,
            Score = 0
        };

        await _attemptRepository.AddAsync(attempt);
        return attempt;
    }

    public async Task<Attempt> FinishAttemptAsync(int attemptId, IEnumerable<AnswerFinishDto> answerDtos)
    {
        var attempt = await _attemptRepository.GetByIdAsync(attemptId);
        if (attempt == null)
            throw new Exception("Attempt not found");

        if (attempt.CompletedAt != DateTime.MinValue)
            throw new Exception("Attempt already completed");

        var questions = await _questionRepository.GetQuestionsByQuizAsync(attempt.QuizId);
        var questionDict = questions.ToDictionary(q => q.Id);

        int correctCount = 0;
        var answersToSave = new List<Answer>();

        foreach (var dto in answerDtos)
        {
            if (!questionDict.TryGetValue(dto.QuestionId, out var question))
                throw new Exception($"Question {dto.QuestionId} does not belong to quiz");

            var userAnswers = dto.UserAnswer?
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList() ?? new List<string>();

            var correctAnswers = question.CorrectAnswer?
                .Select(x => x.Trim())
                .ToList() ?? new List<string>();

            bool isCorrect = correctAnswers.Count == userAnswers.Count &&
                            correctAnswers.All(ca => userAnswers.Contains(ca)) &&
                            userAnswers.All(ua => correctAnswers.Contains(ua));

            var answer = new Answer
            {
                AttemptId = attemptId,
                QuestionId = dto.QuestionId,
                UserAnswer = dto.UserAnswer,
                IsCorrect = isCorrect
            };

            answersToSave.Add(answer);
            if (isCorrect) correctCount++;
        }

        foreach (var answer in answersToSave)
            await _answerRepository.AddAsync(answer);

        attempt.Score = correctCount;
        attempt.CompletedAt = DateTime.UtcNow;

        await _attemptRepository.UpdateAsync(attempt);
        return attempt;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var attempt = await _attemptRepository.GetByIdAsync(id);
        if (attempt == null)
            return false;

        await _attemptRepository.DeleteAsync(id);
        return true;
    }
}
