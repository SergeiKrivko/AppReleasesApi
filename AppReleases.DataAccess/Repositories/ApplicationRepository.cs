using AppReleases.Core.Abstractions;
using AppReleases.Core.Models;
using AppReleases.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppReleases.DataAccess.Repositories;

public class ApplicationRepository(AppReleasesDbContext dbContext) : IApplicationRepository
{
    public async Task<IEnumerable<Application>> GetAllApplicationsAsync()
    {
        var entities = await dbContext.Applications.ToArrayAsync();
        return entities.Select(ApplicationFromEntity);
    }

    public async Task<IEnumerable<Application>> GetAllApplicationsOfUserAsync(Guid userId)
    {
        var entities = await dbContext.Applications.Where(x => x.UserId == userId).ToArrayAsync();
        return entities.Select(ApplicationFromEntity);
    }

    public async Task<Application> GetApplicationByIdAsync(Guid applicationId)
    {
        var entity = await dbContext.Applications.FindAsync(applicationId);
        if (entity == null)
            throw new InvalidOperationException("Application not found");
        return ApplicationFromEntity(entity);
    }

    public async Task<Application> GetApplicationByKeyAsync(string key)
    {
        var entity = await dbContext.Applications
            .Where(x => x.Key == key)
            .SingleAsync();
        return ApplicationFromEntity(entity);
    }

    public async Task<Application> CreateApplicationAsync(Application application)
    {
        var entity = EntityFromApplication(application);
        dbContext.Applications.Add(entity);
        await dbContext.SaveChangesAsync();
        return application;
    }

    public async Task UpdateApplicationAsync(Guid applicationId, string name, string description)
    {
        await dbContext.Applications
            .Where(x => x.ApplicationId == applicationId)
            .ExecuteUpdateAsync(x => x
                .SetProperty(e => e.Name, name)
                .SetProperty(e => e.Description, description)
            );
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteApplicationAsync(Guid applicationId)
    {
        await dbContext.Applications
            .Where(x => x.ApplicationId == applicationId)
            .ExecuteUpdateAsync(x => x
                .SetProperty(e => e.DeletedAt, DateTime.UtcNow)
            );
        await dbContext.SaveChangesAsync();
    }

    private static Application ApplicationFromEntity(ApplicationEntity entity)
    {
        return new Application
        {
            Id = entity.ApplicationId,
            Name = entity.Name,
            Description = entity.Description,
            Key = entity.Key,
            CreatedAt = entity.CreatedAt,
            DeletedAt = entity.DeletedAt,
            OwnerId = entity.UserId,
        };
    }

    private static ApplicationEntity EntityFromApplication(Application application)
    {
        return new ApplicationEntity()
        {
            ApplicationId = application.Id,
            Name = application.Name,
            Description = application.Description,
            Key = application.Key,
            CreatedAt = application.CreatedAt,
            DeletedAt = application.DeletedAt,
            UserId = application.OwnerId,
        };
    }
}