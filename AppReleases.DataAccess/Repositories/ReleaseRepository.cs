using AppReleases.Core.Abstractions;
using AppReleases.Core.Models;
using AppReleases.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppReleases.DataAccess.Repositories;

public class ReleaseRepository(AppReleasesDbContext dbContext) : IReleaseRepository
{
    public async Task<IEnumerable<Release>> GetAllReleasesAsync(Guid branchId)
    {
        var entities = await dbContext.Releases
            .Where(x => x.BranchId == branchId)
            .ToArrayAsync();
        return entities.Select(ReleaseFromEntity);
    }

    public async Task<Release?> GetLatestReleaseAsync(Guid branchId, string platform)
    {
        var entities = await dbContext.Releases
            .Where(x => x.BranchId == branchId && x.Platform == platform)
            .ToArrayAsync();
        return entities
            .Select(ReleaseFromEntity)
            .OrderByDescending(x => x.Version)
            .FirstOrDefault();
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

    public async Task UpdateReleaseAsync(Guid id, string notes)
    {
        await dbContext.Releases.Where(x => x.ReleaseId == id)
            .ExecuteUpdateAsync(x => x
                .SetProperty(e => e.ReleaseNotes, notes)
            );
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteReleaseAsync(Guid id)
    {
        await dbContext.Releases.Where(x => x.ReleaseId == id)
            .ExecuteDeleteAsync();
        await dbContext.SaveChangesAsync();
    }

    private static Release ReleaseFromEntity(ReleaseEntity entity)
    {
        return new Release
        {
            Id = entity.ReleaseId,
            BranchId = entity.BranchId,
            Platform = entity.Platform,
            ReleaseNotes = entity.ReleaseNotes,
            Version = Version.Parse(entity.Version),
            CreatedAt = entity.CreatedAt,
            DeletedAt = entity.DeletedAt,
        };
    }

    private static ReleaseEntity EntityFromRelease(Release release)
    {
        return new ReleaseEntity
        {
            ReleaseId = release.Id,
            BranchId = release.BranchId,
            Platform = release.Platform,
            ReleaseNotes = release.ReleaseNotes,
            Version = release.Version.ToString(),
            CreatedAt = release.CreatedAt,
            DeletedAt = release.DeletedAt,
        };
    }
}