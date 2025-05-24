using AppReleases.Core.Models;

namespace AppReleases.Core.Abstractions;

public interface IAssetRepository
{
    public Task<IEnumerable<Asset>> GetAllAssetsAsync(Guid releaseId);
    public Task<Asset> GetAssetByIdAsync(Guid assetId);
    public Task<Asset> CreateAssetAsync(Asset asset);
}