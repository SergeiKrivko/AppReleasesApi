using AppReleases.Models;

namespace AppReleases.Core.Abstractions;

public interface ICleanerService
{
    public Task<int> ClearOldReleasesAsync(CancellationToken cancellationToken);
    public Task<int> ClearOldReleasesOfApplicationAsync(Guid applicationId);
    public Task<int> ClearOldReleasesOfApplicationAsync(Application application);
    public Task<int> ClearOldReleasesOfBranchAsync(Guid branchId);
    public Task<int> ClearOldReleasesOfBranchAsync(Application application, Branch branch);
    public Task<int> ClearUnusedAssetsAsync(CancellationToken cancellationToken);
    public Task<int> ClearOldInstallersAsync(CancellationToken cancellationToken);
    public Task<int> ClearOldInstallersOfApplicationAsync(Guid applicationId, CancellationToken cancellationToken);
}