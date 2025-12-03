using System.Text.Json;
using System.Text.Json.Nodes;
using AppReleases.Core.Abstractions;
using AppReleases.DataAccess.Entities;
using AppReleases.Models;
using Microsoft.EntityFrameworkCore;

namespace AppReleases.DataAccess.Repositories;

public class InstallerBuilderRepository(AppReleasesDbContext dbContext) : IInstallerBuilderRepository
{
    public async Task<IEnumerable<InstallerBuilderUsage>> GetInstallerBuildersOfApplicationAsync(Guid applicationId)
    {
        var entities = await dbContext.InstallerBuilderUsage
            .Where(x => x.ApplicationId == applicationId)
            .ToListAsync();
        return entities.Select(UsageFromEntity);
    }

    private static InstallerBuilderUsage UsageFromEntity(InstallerBuilderUsageEntity entity)
    {
        return new InstallerBuilderUsage
        {
            BuilderKey = entity.BuilderKey,
            Settings = entity.Settings is null
                ? new JsonObject()
                : JsonSerializer.Deserialize<JsonObject>(entity.Settings) ?? new JsonObject(),
        };
    }
}