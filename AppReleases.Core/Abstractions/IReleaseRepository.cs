using AppReleases.Models;

namespace AppReleases.Core.Abstractions;

public interface IReleaseRepository
{
    public Task<IEnumerable<Release>> GetAllReleasesOfBranchAsync(Guid branchId);
    public Task<IEnumerable<Release>> GetAllReleasesOfApplicationAsync(Guid applicationId);
    public Task<Release> GetReleaseByIdAsync(Guid id);
    public Task<Release?> GetLatestReleaseAsync(Guid branchId, string platform);
    public Task<Release> CreateReleaseAsync(Release release);
    public Task UpdateReleaseAsync(Guid id, string? notes);
    public Task DeleteReleaseAsync(Guid id);
    public Task<Release?> FindReleaseAsync(Guid branchId, string? platform, Version version);
}