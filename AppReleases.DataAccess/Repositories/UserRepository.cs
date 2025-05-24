using AppReleases.Core.Abstractions;
using AppReleases.Core.Models;
using AppReleases.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppReleases.DataAccess.Repositories;

public class UserRepository(AppReleasesDbContext dbContext) : IUserRepository
{
    public async Task<User> GetUserByIdAsync(Guid id)
    {
        var entity = await dbContext.Users.FindAsync(id);
        if (entity == null)
            throw new InvalidOperationException($"User '{id}' not found");
        return UserFromEntity(entity);
    }

    public async Task<User> GetUserByLoginAsync(string login)
    {
        var entity = await dbContext.Users.SingleAsync(x => x.Login == login);
        return UserFromEntity(entity);
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        var entities = await dbContext.Users.ToArrayAsync();
        return entities.Select(UserFromEntity);
    }

    public async Task<User> CreateUserAsync(string login, string passwordHash)
    {
        var entity = new UserEntity
        {
            UserId = Guid.NewGuid(),
            Login = login,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow
        };
        await dbContext.AddAsync(entity);
        await dbContext.SaveChangesAsync();
        return UserFromEntity(entity);
    }

    public async Task UpdateUserAsync(Guid userId, string username, string passwordHash)
    {
        await dbContext.Users.Where(x => x.UserId == userId)
            .ExecuteUpdateAsync(o => o
                .SetProperty(e => e.Login, username)
                .SetProperty(e => e.PasswordHash, passwordHash)
            );
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(Guid userId)
    {
        await dbContext.Users.Where(x => x.UserId == userId)
            .ExecuteUpdateAsync(o => o
                .SetProperty(e => e.DeletedAt, DateTime.UtcNow)
            );
        await dbContext.SaveChangesAsync();
    }

    private static User UserFromEntity(UserEntity entity)
    {
        return new User
        {
            Id = entity.UserId,
            Login = entity.Login,
            PasswordHash = entity.PasswordHash,
            CreatedAt = entity.CreatedAt,
            DeletedAt = entity.DeletedAt,
        };
    }
}