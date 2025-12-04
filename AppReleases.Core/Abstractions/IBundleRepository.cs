using AppReleases.Models;

namespace AppReleases.Core.Abstractions;

public interface IBundleRepository
{
    public Task<Bundle> GetBundleByIdAsync(Guid id);
    public Task<IEnumerable<Bundle>> GetAllBundlesAsync(Guid releaseId);
    public Task<Bundle?> FindBundleAsync(Guid releaseId, string fileName);
    public Task CreateBundleAsync(Bundle bundle);
    public Task DeleteBundleAsync(Guid bundleId);
}