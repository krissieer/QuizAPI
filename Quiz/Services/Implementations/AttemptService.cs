using Quiz.DTOs.Attempt;
using Quiz.Models;
using Quiz.Repositories.Interfaces;
using Quiz.Services.Interfaces;
using System.Security.Claims;

namespace Quiz.Services.Implementations;

public class AttemptService : IAttemptService
{
    private readonly IAttemptRepository _attemptRepository;
    private readonly IQuizRepository _quizRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly IOptionRepository _optionRepository;
    private readonly IUserAnswerRepository _userAnswerRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AttemptService(
        IAttemptRepository attemptRepository,
        IQuizRepository quizRepository,
        IQuestionRepository questionRepository,
        IOptionRepository optionRepository,
        IUserAnswerRepository userAnswerRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _attemptRepository = attemptRepository;
        _quizRepository = quizRepository;
        _questionRepository = questionRepository;
        _optionRepository = optionRepository;
        _userAnswerRepository = userAnswerRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Attempt?> GetByIdAsync(int id)
        => await _attemptRepository.GetByIdWithDetailsAsync(id);

    public async Task<IEnumerable<Attempt>> GetByUserIdAsync(int userId)
        => await _attemptRepository.GetAttemptsByUserAsync(userId);

    public async Task<IEnumerable<Attempt>> GetByQuizIdAsync(int quizId)
        => await _attemptRepository.GetAttemptsByQuizAsync(quizId);

    public async Task<Attempt> StartAttemptAsync(int quizId)
    {
        var quiz = await _quizRepository.GetByIdAsync(quizId)
            ?? throw new KeyNotFoundException("Quiz not found");

        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        int? userId = int.TryParse(userIdClaim, out var uid) ? uid : null;
        var guestId = _httpContextAccessor.HttpContext?.Items["GuestSessionId"]?.ToString();

        var attempt = new Attempt
        {
            QuizId = quizId,
            UserId = userId,
            GuestSessionId = userId == null ? guestId : null,
            Score = 0,
            TimeSpent = TimeSpan.Zero,
            CompletedAt = DateTime.UtcNow
        };

        await _attemptRepository.AddAsync(attempt);
        return attempt;
    }

    public async Task<Attempt> FinishAttemptAsync(int attemptId, IEnumerable<AnswerFinishDto> answerDtos)
    {
        var attempt = await _attemptRepository.GetByIdAsync(attemptId)
            ?? throw new KeyNotFoundException("Attempt not found");

        if (attempt.TimeSpent != TimeSpan.Zero)
            throw new InvalidOperationException("Attempt already completed");

        var questions = await _questionRepository.GetQuestionsWithOptionsByQuizAsync(attempt.QuizId);
        var questionDict = questions.ToDictionary(q => q.Id);

        int correctCount = 0;
        var userAnswersToSave = new List<UserAnswer>();

        foreach (var dto in answerDtos)
        {
            if (!questionDict.TryGetValue(dto.QuestionId, out var question))
                throw new InvalidOperationException($"Question {dto.QuestionId} not found in quiz");

            var selectedOptionIds = dto.SelectedOptionIds?.ToHashSet() ?? new HashSet<int>();

            // Получаем все правильные варианты ответа для вопроса
            var correctOptionIds = question.Options
                .Where(o => o.IsCorrect)
                .Select(o => o.Id)
                .ToHashSet();

            bool isCorrect = selectedOptionIds.SetEquals(correctOptionIds);

            if (isCorrect) correctCount++;

            // Сохраняем каждый выбранный вариант (для Multiple — может быть несколько)
            foreach (var optionId in selectedOptionIds)
            {
                userAnswersToSave.Add(new UserAnswer
                {
                    AttemptId = attemptId,
                    QuestionId = dto.QuestionId,
                    ChosenOptionId = optionId
                });
            }
        }

        // Сохраняем ответы
        foreach (var ua in userAnswersToSave)
            await _userAnswerRepository.AddAsync(ua);

        // Обновляем попытку
        attempt.Score = correctCount;
        attempt.TimeSpent = DateTime.UtcNow - attempt.CompletedAt;
        attempt.CompletedAt = DateTime.UtcNow;

        await _attemptRepository.UpdateAsync(attempt);
        return attempt;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await _attemptRepository.DeleteAsync(id);
        return true;
    }
}