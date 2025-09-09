using AppReleases.Core.Abstractions;
using AppReleases.Models;
using AppReleases.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppReleases.DataAccess.Repositories;

public class AssetRepository(AppReleasesDbContext dbContext) : IAssetRepository
{
    public async Task<IEnumerable<Asset>> GetAllAssetsAsync(Guid releaseId)
    {
        var entities = await dbContext.Releases
            .Where(x => x.ReleaseId == releaseId)
            .Include(x => x.Assets)
            .Where(x => x.ReleaseId == releaseId)
            .Select(x => x.Assets)
            .SingleAsync();
        return entities.Select(AssetFromEntity);
    }

    public async Task<Asset> GetAssetByIdAsync(Guid assetId)
    {
        var entity = await dbContext.Assets
            .Where(x => x.AssetId == assetId)
            .SingleAsync();
        return AssetFromEntity(entity);
    }

    public async Task<Asset> CreateAssetAsync(Asset asset)
    {
        var entity = EntityFromAsset(asset);
        await dbContext.Assets.AddAsync(entity);
        await dbContext.SaveChangesAsync();
        return asset;
    }

    public async Task<Asset?> FindAssetAsync(string fileName, string fileHash)
    {
        var entity = await dbContext.Assets
            .Where(x => x.FileName == fileName && x.FileHash == fileHash && x.DeletedAt == null)
            .SingleOrDefaultAsync();
        return entity is null ? null : AssetFromEntity(entity);
    }

    public async Task DeleteAssetAsync(Guid assetId)
    {
        await dbContext.Assets
            .Where(x => x.AssetId == assetId)
            .ExecuteUpdateAsync(x => x.SetProperty(e => e.DeletedAt, DateTime.UtcNow));
        await dbContext.SaveChangesAsync();
    }

    public async Task AddAssetToReleaseAsync(Guid assetId, Guid releaseId)
    {
        await dbContext.ReleaseAssets.AddAsync(new ReleaseAssetEntity
        {
            ReleaseId = releaseId,
            AssetId = assetId,
            Id = Guid.NewGuid(),
        });
        await dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<Asset>> GetUnusedAssetsAsync()
    {
        var assets = await dbContext.Assets
            .Include(e => e.Releases)
            .Where(e => e.DeletedAt == null && e.Releases.All(r => r.DeletedAt != null))
            .ToArrayAsync();
        return assets.Select(AssetFromEntity);
    }

    private static Asset AssetFromEntity(AssetEntity entity)
    {
        return new Asset
        {
            Id = entity.AssetId,
            FileName = entity.FileName,
            FileHash = entity.FileHash,
            FileId = entity.FileId,
            CreatedAt = entity.CreatedAt,
        };
    }

    private static AssetEntity EntityFromAsset(Asset asset)
    {
        return new AssetEntity
        {
            AssetId = asset.Id,
            FileName = asset.FileName,
            FileHash = asset.FileHash,
            FileId = asset.FileId,
            CreatedAt = asset.CreatedAt,
        };
    }
}