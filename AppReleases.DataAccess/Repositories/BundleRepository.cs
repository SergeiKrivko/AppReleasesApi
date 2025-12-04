using AppReleases.Core.Abstractions;
using AppReleases.DataAccess.Entities;
using AppReleases.Models;
using Microsoft.EntityFrameworkCore;

namespace AppReleases.DataAccess.Repositories;

public class BundleRepository(AppReleasesDbContext dbContext) : IBundleRepository
{
    public async Task<Bundle> GetBundleByIdAsync(Guid id)
    {
        var result = await dbContext.Bundles
            .Where(x => x.BundleId == id)
            .FirstAsync();
        return BundleFromEntity(result);
    }

    public async Task<IEnumerable<Bundle>> GetAllBundlesAsync(Guid releaseId)
    {
        var result = await dbContext.Bundles
            .Where(x => x.ReleaseId == releaseId)
            .ToListAsync();
        return result.Select(BundleFromEntity);
    }

    public async Task<Bundle?> FindBundleAsync(Guid releaseId, string fileName)
    {
        var result = await dbContext.Bundles
            .Where(x => x.ReleaseId == releaseId && x.FileName == fileName)
            .SingleOrDefaultAsync();
        return result is null ? null : BundleFromEntity(result);
    }

    public async Task CreateBundleAsync(Bundle bundle)
    {
        var entity = BundleToEntity(bundle);
        await dbContext.Bundles.AddAsync(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteBundleAsync(Guid bundleId)
    {
        await dbContext.Bundles
            .Where(x => x.BundleId == bundleId)
            .ExecuteUpdateAsync(x => x.SetProperty(e => e.DeletedAt, DateTime.UtcNow));
    }

    private static Bundle BundleFromEntity(BundleEntity entity)
    {
        return new Bundle
        {
            BundleId = entity.BundleId,
            ReleaseId = entity.ReleaseId,
            FileName = entity.FileName,
            CreatedAt = entity.CreatedAt,
            DeletedAt = entity.DeletedAt,
            FileId = entity.FileId,
        };
    }

    private static BundleEntity BundleToEntity(Bundle bundle)
    {
        return new BundleEntity
        {
            BundleId = bundle.BundleId,
            ReleaseId = bundle.ReleaseId,
            FileName = bundle.FileName,
            CreatedAt = bundle.CreatedAt,
            DeletedAt = bundle.DeletedAt,
            FileId = bundle.FileId,
        };
    }
}