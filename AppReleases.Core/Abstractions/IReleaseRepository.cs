using AppReleases.Core.Models;

namespace AppReleases.Core.Abstractions;

public interface IReleaseRepository
{
    public Task<IEnumerable<Release>> GetAllReleasesAsync(Guid branchId);
    public Task<Release> GetReleaseByIdAsync(Guid id);
    public Task<Release?> GetLatestReleaseAsync(Guid branchId, string platform);
    public Task<Release> CreateReleaseAsync(Release release);
    public Task UpdateReleaseAsync(Guid id, string notes);
    public Task DeleteReleaseAsync(Guid id);
}