using Quiz.Models;
using Quiz.Repositories.Interfaces;
using Quiz.Services.Interfaces;
using System.Xml.Linq;

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

    public async Task<User> RegisterAsync(string username, string password, Role role)
    {
        var hashedPassword = PasswordHasher.HashPassword(password);
        var existing = await _userRepository.GetByUsernameAsync(username);
        if (existing != null)
            throw new Exception("User with this username already exists");

        var user = new User
        {
            Username = username,
            PasswordHash = hashedPassword,
            CreatedAt = DateTime.UtcNow,
            Role = role
        };

        await _userRepository.AddAsync(user);
        return user;
    }

    public async Task<User?> LoginAsync(string username, string password)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null)
            return null;

        if (!PasswordHasher.VerifyPassword(password, user.PasswordHash))
            return null;
        else return user;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return false;

        await _userRepository.DeleteAsync(user);
        return true;
    }
}
