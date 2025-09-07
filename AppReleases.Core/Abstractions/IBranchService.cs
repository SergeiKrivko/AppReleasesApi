using AppReleases.Core.Models;

namespace AppReleases.Core.Abstractions;

public interface IBranchService
{
    public Task<IEnumerable<Branch>> GetAllBranchesAsync(Guid applicationId);
    public Task<Branch> GetBranchByIdAsync(Guid branchId);
    public Task<Branch?> GetBranchByNameAsync(Guid applicationId, string name);
    public Task<Branch> CreateBranchAsync(Guid applicationId, string name, TimeSpan? duration, bool useDefaultDuration);
    public Task UpdateBranchAsync(Guid branchId, TimeSpan? duration, bool useDefaultDuration);
    public Task DeleteBranchAsync(Guid branchId);
    public Task<Branch> GetOrCreateBranchAsync(Guid applicationId, string name);
}