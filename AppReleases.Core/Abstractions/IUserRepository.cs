using AppReleases.Core.Models;

namespace AppReleases.Core.Abstractions;

public interface IUserRepository
{
    public Task<User> GetUserByIdAsync(Guid id);
    public Task<User> GetUserByLoginAsync(string name);
    public Task<IEnumerable<User>> GetAllUsersAsync();
    public Task<User> CreateUserAsync(string login, string passwordHash);
    public Task UpdateUserAsync(Guid userId, string username, string passwordHash);
    public Task DeleteUserAsync(Guid userId);
}