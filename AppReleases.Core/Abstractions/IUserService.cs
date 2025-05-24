using AppReleases.Core.Models;

namespace AppReleases.Core.Abstractions;

public interface IUserService
{
    public Task<User> FindUserAsync(string username);
    public Task CreateUserAsync(string username, string password);
    public Task UpdateUserAsync(Guid userId, string username, string password);
    public Task UpdateUserAsync(string username, string password);
    public Task<User?> VerifyUserAsync(string username, string password);
    public Task DeleteUserAsync(string username);
    public Task DeleteUserAsync(Guid userId);
}