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

    public async Task UpdateApplicationAsync(Guid applicationId, string name, string description, string mainBranch,
        TimeSpan? defaultDuration = null)
    {
        await dbContext.Applications
            .Where(x => x.ApplicationId == applicationId)
            .ExecuteUpdateAsync(x => x
                .SetProperty(e => e.Name, name)
                .SetProperty(e => e.Description, description)
                .SetProperty(e => e.MainBranch, mainBranch)
                .SetProperty(e => e.DefaultDuration, defaultDuration)
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
            MainBranch = entity.MainBranch,
            DefaultDuration = entity.DefaultDuration,
            CreatedAt = entity.CreatedAt,
            DeletedAt = entity.DeletedAt,
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
            MainBranch = application.MainBranch,
            DefaultDuration = application.DefaultDuration,
            CreatedAt = application.CreatedAt,
            DeletedAt = application.DeletedAt,
        };
    }
}