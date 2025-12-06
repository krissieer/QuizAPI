using Quiz.Models;
using Quiz.Repositories.Interfaces;
using Quiz.Services.Interfaces;
using System.Xml.Linq;
using Quiz.DTOs.User;

namespace Quiz.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
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
            //Role = Role.Authorized
        };

        await _userRepository.AddAsync(user);
        return TokenGeneration.GenerateToken(user.Id);
    }

    public async Task<string> LoginAsync(string username, string password)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null)
            return string.Empty;

        if (!PasswordHasher.VerifyPassword(password, user.PasswordHash))
            return string.Empty;

        return TokenGeneration.GenerateToken(user.Id);
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
