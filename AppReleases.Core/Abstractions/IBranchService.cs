using AppReleases.Models;

namespace AppReleases.Core.Abstractions;

public interface IBranchService
{
    public Task<IEnumerable<Branch>> GetAllBranchesAsync(Guid applicationId);
    public Task<Branch> GetBranchByIdAsync(Guid branchId);
    public Task<Branch?> GetBranchByNameAsync(Guid applicationId, string name);

    public Task<Branch> CreateBranchAsync(Guid applicationId, string name, TimeSpan? releaseLifetime,
        TimeSpan? latestReleaseLifetime, bool useDefaultReleaseLifetime);

    public Task UpdateBranchAsync(Guid branchId, TimeSpan? releaseLifetime,
        TimeSpan? latestReleaseLifetime, bool useDefaultReleaseLifetime);

    public Task DeleteBranchAsync(Guid branchId);
    public Task<Branch> GetOrCreateBranchAsync(Guid applicationId, string name);
}