using Quiz.Models;

namespace Quiz.Services.Interfaces;

public interface IUserService
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByUsernameAsync(string username);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> RegisterAsync(string username, string password, Role role);
    Task<User?> LoginAsync(string username, string password);
    Task<bool> DeleteAsync(int id);
}
