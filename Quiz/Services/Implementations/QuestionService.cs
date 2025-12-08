using Quiz.Models;
using Quiz.Repositories.Interfaces;
using Quiz.Services.Interfaces;
using Quiz.DTOs.Question;

namespace Quiz.Services.Implementations;

public class QuestionService : IQuestionService
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IQuizRepository _quizRepository;
    private readonly IOptionRepository _optionRepository;

    public QuestionService(
        IQuestionRepository questionRepository,
        IQuizRepository quizRepository,
        IOptionRepository optionRepository)
    {
        _questionRepository = questionRepository;
        _quizRepository = quizRepository;
        _optionRepository = optionRepository;
    }

    public async Task<Question?> GetByIdAsync(int id)
        => await _questionRepository.GetByIdWithOptionsAsync(id);

    public async Task<IEnumerable<Question>> GetByQuizAsync(int quizId)
        => await _questionRepository.GetQuestionsWithOptionsByQuizAsync(quizId);

    public async Task<Question> CreateAsync(Question question, List<string> optionTexts, List<bool> isCorrectFlags)
    {
        if (optionTexts.Count != isCorrectFlags.Count)
            throw new ArgumentException("Options and correctness flags count must match");

        ValidateOptions(question, isCorrectFlags);

        var quiz = await _quizRepository.GetByIdAsync(question.QuizId)
            ?? throw new KeyNotFoundException("Quiz not found");

        await _questionRepository.AddAsync(question);

        // Создаём опции
        var options = new List<Option>();
        for (int i = 0; i < optionTexts.Count; i++)
        {
            options.Add(new Option
            {
                Text = optionTexts[i],
                QuestionId = question.Id,
                IsCorrect = isCorrectFlags[i]
            });
        }

        await _optionRepository.AddRangeAsync(options);

        question.Options = options;
        return question;
    }

    public async Task<bool> UpdateAsync(Question question, List<string>? optionTexts = null, List<bool>? isCorrectFlags = null)
    {
        var existing = await _questionRepository.GetByIdWithOptionsAsync(question.Id)
            ?? throw new KeyNotFoundException("Question not found");

        existing.Text = question.Text;
        existing.Type = question.Type;

        // Если передали новые опции — обновляем
        if (optionTexts != null && isCorrectFlags != null)
        {
            if (optionTexts.Count != isCorrectFlags.Count)
                throw new ArgumentException("Options and flags count must match");
            
            ValidateOptions(existing, isCorrectFlags);

            // Удаляем старые
            foreach (var opt in existing.Options.ToList())
                await _optionRepository.DeleteAsync(opt.Id);

            // Добавляем новые
            var newOptions = new List<Option>();
            for (int i = 0; i < optionTexts.Count; i++)
            {
                newOptions.Add(new Option
                {
                    Text = optionTexts[i],
                    QuestionId = question.Id,
                    IsCorrect = isCorrectFlags[i]
                });
            }
            await _optionRepository.AddRangeAsync(newOptions);
            existing.Options = newOptions;
        }

        await _questionRepository.UpdateAsync(existing);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _questionRepository.GetByIdAsync(id);
        if (existing == null)
            return false;

        await _questionRepository.DeleteAsync(id);
        return true;
    }
    
    private void ValidateOptions(Question question, List<bool> isCorrectFlags)
    {
        // 1. Проверка минимального количества опций
        if (isCorrectFlags.Count < 2)
        {
            throw new InvalidOperationException("Question must have at least two options.");
        }

        // Считаем количество правильных опций
        int correctCount = isCorrectFlags.Count(isCorrect => isCorrect);

        // 2. Валидация в зависимости от типа вопроса
        if (question.Type == QuestionType.Single)
        {
            // Для одиночного выбора должно быть ровно 1 правильная опция
            if (correctCount != 1)
            {
                throw new InvalidOperationException("Single-choice question must have exactly one correct option.");
            }
        }
        else if (question.Type == QuestionType.Multiple)
        {
            // Для множественного выбора должна быть хотя бы 2 правильные опции
            if (correctCount < 2)
            {
                throw new InvalidOperationException("Multiple-choice question must have at least two corrects option.");
            }
        }
    }
}