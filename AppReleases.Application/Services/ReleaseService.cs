using AppReleases.Core.Abstractions;
using AppReleases.Core.Models;

namespace AppReleases.Application.Services;

public class ReleaseService(IReleaseRepository releaseRepository) : IReleaseService
{
    public Task<Release> GetReleaseByIdAsync(Guid releaseId)
    {
        return releaseRepository.GetReleaseByIdAsync(releaseId);
    }

    public Task<Release?> GetLatestReleaseAsync(Guid branchId, string platform)
    {
        return releaseRepository.GetLatestReleaseAsync(branchId, platform);
    }
}