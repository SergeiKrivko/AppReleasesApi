using AppReleases.Core.Abstractions;
using AppReleases.Core.Models;
using AppReleases.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppReleases.DataAccess.Repositories;

public class AssetRepository(AppReleasesDbContext dbContext) : IAssetRepository
{
    public async Task<IEnumerable<Asset>> GetAllAssetsAsync(Guid releaseId)
    {
        var entities = await dbContext.Assets
            .Where(x => x.ReleaseId == releaseId)
            .ToArrayAsync();
        return entities.Select(AssetFromEntity);
    }

    public async Task<Asset> GetAssetByIdAsync(Guid assetId)
    {
        var entity = await dbContext.Assets
            .Where(x => x.ReleaseId == assetId)
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

    private static Asset AssetFromEntity(AssetEntity entity)
    {
        return new Asset
        {
            Id = entity.AssetId,
            ReleaseId = entity.ReleaseId,
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
            ReleaseId = asset.ReleaseId,
            FileName = asset.FileName,
            FileHash = asset.FileHash,
            FileId = asset.FileId,
            CreatedAt = asset.CreatedAt,
        };
    }
}