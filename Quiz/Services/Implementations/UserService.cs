using Quiz.DTOs.User;
using Quiz.Models;
using Quiz.Repositories.Implementations;
using Quiz.Repositories.Interfaces;
using Quiz.Services.Interfaces;
using System.Xml.Linq;

namespace Quiz.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILoginAttemptRepository _loginAttemptRepository;

    public UserService(IUserRepository userRepository, ILoginAttemptRepository loginAttemptRepository)
    {
        _userRepository = userRepository;
        _loginAttemptRepository = loginAttemptRepository;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _userRepository.GetByUsernameAsync(username);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task<string> RegisterAsync(string username, string password)
    {
        var existing = await _userRepository.GetByUsernameAsync(username);
        if (existing != null)
            return string.Empty;

        var hashedPassword = PasswordHasher.HashPassword(password);

        var user = new User
        {
            Username = username,
            PasswordHash = hashedPassword,
            CreatedAt = DateTime.UtcNow,
        };

        await _userRepository.AddAsync(user);
        return TokenGeneration.GenerateToken(user.Id);
    }

    public async Task<string> LoginAsync(string username, string password)
    {
        var attempt = await _loginAttemptRepository.GetByUsernameAsync(username);
        if (attempt != null && attempt.IsLocked)
            throw new Exception("Account temporarily locked due to too many failed login attempts. Try again later.");

        var user = await _userRepository.GetByUsernameAsync(username);
        //if (user == null)
        //    return string.Empty;

        if (user == null || !PasswordHasher.VerifyPassword(password, user.PasswordHash))
        {
            // увеличиваем счетчик
            if (attempt == null)
                attempt = new LoginAttempt 
                {
                    Username = username, 
                    AttemptCount = 1, 
                    LastAttempt = DateTime.UtcNow 
                };
            else
            {
                attempt.AttemptCount++;
                attempt.LastAttempt = DateTime.UtcNow;
            }

            await _loginAttemptRepository.AddOrUpdateAsync(attempt);
            return string.Empty;
        }

        // успешный вход — сбрасываем счетчик
        if (attempt != null)
            await _loginAttemptRepository.ResetAttemptsAsync(username);

        // возвращаем токен
        return TokenGeneration.GenerateToken(user.Id);

       
        //if (!PasswordHasher.VerifyPassword(password, user.PasswordHash))
        //    return string.Empty;

        //return TokenGeneration.GenerateToken(user.Id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return false;

        await _userRepository.DeleteAsync(id);
        return true;
    }

    public async Task<bool> UpdateAsync(User user)
    {
        var existingUser = await _userRepository.GetByIdAsync(user.Id);
        if (existingUser == null)
            return false;

        existingUser.Username = user.Username;
        existingUser.PasswordHash = user.PasswordHash;

        await _userRepository.UpdateAsync(existingUser);
        return true;
    }
}
