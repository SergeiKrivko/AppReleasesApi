using AppReleases.Core.Abstractions;
using AppReleases.Core.Models;

namespace AppReleases.Application.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    public Task<User> FindUserAsync(string username)
    {
        return userRepository.GetUserByLoginAsync(username);
    }

    public Task CreateUserAsync(string username, string password)
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        return userRepository.CreateUserAsync(username, passwordHash);
    }

    public Task UpdateUserAsync(Guid userId, string username, string password)
    {
        return userRepository.UpdateUserAsync(userId, username, password);
    }

    public async Task UpdateUserAsync(string username, string password)
    {
        var user = await FindUserAsync(username);
        await UpdateUserAsync(user.Id, username, password);
    }

    public async Task<User?> VerifyUserAsync(string username, string password)
    {
        var user = await FindUserAsync(username);
        if (BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return user;
        return null;
    }

    public async Task DeleteUserAsync(string username)
    {
        var user = await FindUserAsync(username);
        await DeleteUserAsync(user.Id);
    }

    public Task DeleteUserAsync(Guid userId)
    {
        return userRepository.DeleteUserAsync(userId);
    }
}