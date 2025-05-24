using AppReleases.Core.Models;

namespace AppReleases.Core.Abstractions;

public interface IReleaseRepository
{
    public Task<IEnumerable<Release>> GetAllReleasesAsync(Guid applicationId);
    public Task<Release> GetReleaseByIdAsync(Guid id);
    public Task<Release> GetLatestReleaseAsync(Guid applicationId, bool includeBranches = false);
    public Task<Release> GetLatestReleaseForBranchAsync(Guid applicationId, Guid branchId);
    public Task<Release> CreateReleaseAsync(Release release);
    public Task UpdateReleaseAsync(Guid id, string notes, bool isObsolete = false);
}