using AppReleases.Core.Abstractions;
using AppReleases.Core.Models;
using AppReleases.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppReleases.DataAccess.Repositories;

public class ReleaseRepository(AppReleasesDbContext dbContext) : IReleaseRepository
{
    public async Task<IEnumerable<Release>> GetAllReleasesAsync(Guid applicationId)
    {
        var entities = await dbContext.Releases.ToArrayAsync();
        return entities.Select(ReleaseFromEntity);
    }

    public async Task<Release> GetLatestReleaseAsync(Guid applicationId, bool includeBranches = false)
    {
        ReleaseEntity entity;
        if (includeBranches)
        {
            entity = await dbContext.Releases
                .Where(x => x.ApplicationId == applicationId)
                .OrderBy(x => x.CreatedAt)
                .LastAsync();
        }
        else
        {
            entity = await dbContext.Releases
                .Where(x => x.ApplicationId == applicationId)
                .Where(x => x.BranchId == null)
                .OrderBy(x => x.CreatedAt)
                .LastAsync();
        }
        return ReleaseFromEntity(entity);
    }

    public async Task<Release> GetLatestReleaseForBranchAsync(Guid applicationId, Guid branchId)
    {
        var entity = await dbContext.Releases
            .Where(x => x.ApplicationId == applicationId && x.BranchId == branchId)
            .OrderBy(x => x.CreatedAt)
            .LastAsync();
        return ReleaseFromEntity(entity);
    }

    public async Task<Release> GetReleaseByIdAsync(Guid id)
    {
        var entity = await dbContext.Releases.Where(x => x.ReleaseId == id).SingleAsync();
        return ReleaseFromEntity(entity);
    }

    public async Task<Release> CreateReleaseAsync(Release release)
    {
        var entity = EntityFromRelease(release);
        dbContext.Releases.Add(entity);
        await dbContext.SaveChangesAsync();
        return release;
    }

    public async Task UpdateReleaseAsync(Guid id, string notes, bool isObsolete = false)
    {
        await dbContext.Releases.Where(x => x.ReleaseId == id)
            .ExecuteUpdateAsync(x => x
                .SetProperty(e => e.ReleaseNotes, notes)
                .SetProperty(e => e.IsObsolete, isObsolete)
            );
        await dbContext.SaveChangesAsync();
    }

    private static Release ReleaseFromEntity(ReleaseEntity entity)
    {
        return new Release
        {
            Id = entity.ReleaseId,
            ApplicationId = entity.ApplicationId,
            CreatedAt = entity.CreatedAt,
            Platform = entity.Platform,
            ReleaseNotes = entity.ReleaseNotes,
            Version = Version.Parse(entity.Version),
            IsObsolete = entity.IsObsolete,
            IsPrerelease = entity.IsPrerelease,
        };
    }

    private static ReleaseEntity EntityFromRelease(Release release)
    {
        return new ReleaseEntity
        {
            ReleaseId = release.Id,
            ApplicationId = release.ApplicationId,
            CreatedAt = release.CreatedAt,
            Platform = release.Platform,
            ReleaseNotes = release.ReleaseNotes,
            Version = release.Version.ToString(),
            IsObsolete = release.IsObsolete,
            IsPrerelease = release.IsPrerelease,
        };
    }
}