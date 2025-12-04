using System.Text.Json;
using System.Text.Json.Nodes;
using AppReleases.Core.Abstractions;
using AppReleases.DataAccess.Entities;
using AppReleases.Models;
using Microsoft.EntityFrameworkCore;

namespace AppReleases.DataAccess.Repositories;

public class InstallerBuilderRepository(AppReleasesDbContext dbContext) : IInstallerBuilderRepository
{
    public async Task<IEnumerable<InstallerBuilderUsage>> GetAllInstallerBuildersOfApplicationAsync(Guid applicationId,
        CancellationToken cancellationToken = default)
    {
        var entities = await dbContext.InstallerBuilderUsages
            .Where(x => x.ApplicationId == applicationId)
            .ToListAsync(cancellationToken);
        return entities.Select(UsageFromEntity);
    }

    public async Task<InstallerBuilderUsage?> GetInstallerBuilderByIdAsync(Guid builderId,
        CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.InstallerBuilderUsages
            .FirstOrDefaultAsync(x => x.Id == builderId, cancellationToken);
        return entity is null ? null : UsageFromEntity(entity);
    }

    public async Task<Guid> CreateInstallerBuilderForApplicationAsync(Guid applicationId, string builderKey,
        string? name,
        TimeSpan installerLifetime, string[] platforms,
        CancellationToken cancellationToken = default)
    {
        var id = Guid.NewGuid();
        var entity = new InstallerBuilderUsageEntity
        {
            ApplicationId = applicationId,
            BuilderKey = builderKey,
            Id = id,
            Name = name,
            Platforms = platforms,
            CreatedAt = DateTime.UtcNow,
            InstallerLifetime = installerLifetime,
        };
        await dbContext.InstallerBuilderUsages.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return id;
    }

    public async Task DeleteInstallerBuilderAsync(Guid builderId, CancellationToken cancellationToken = default)
    {
        await dbContext.InstallerBuilderUsages
            .Where(e => e.Id == builderId)
            .ExecuteUpdateAsync(x => x
                    .SetProperty(e => e.DeletedAt, DateTime.UtcNow),
                cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateInstallerBuilderAsync(Guid builderId, string? name, TimeSpan installerLifetime,
        string[] platforms,
        CancellationToken cancellationToken = default)
    {
        await dbContext.InstallerBuilderUsages
            .Where(e => e.Id == builderId)
            .ExecuteUpdateAsync(x => x
                    .SetProperty(e => e.Name, name)
                    .SetProperty(e => e.InstallerLifetime, installerLifetime)
                    .SetProperty(e => e.Platforms, platforms),
                cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static InstallerBuilderUsage UsageFromEntity(InstallerBuilderUsageEntity entity)
    {
        return new InstallerBuilderUsage
        {
            Id = entity.Id,
            BuilderKey = entity.BuilderKey,
            Name = entity.Name,
            Platforms = entity.Platforms,
            Settings = entity.Settings is null
                ? new JsonObject()
                : JsonSerializer.Deserialize<JsonObject>(entity.Settings) ?? new JsonObject(),
            InstallerLifetime = entity.InstallerLifetime,
        };
    }
}