using AppReleases.Core.Abstractions;
using AppReleases.Models;

namespace AppReleases.Application.Services;

public class BranchService(IBranchRepository branchRepository) : IBranchService
{
    public Task<IEnumerable<Branch>> GetAllBranchesAsync(Guid applicationId)
    {
        return branchRepository.GetAllBranchesAsync(applicationId);
    }

    public Task<Branch> GetBranchByIdAsync(Guid branchId)
    {
        return branchRepository.GetBranchByIdAsync(branchId);
    }

    public Task<Branch?> GetBranchByNameAsync(Guid applicationId, string name)
    {
        return branchRepository.GetBranchByNameAsync(applicationId, name);
    }

    public async Task<Branch> CreateBranchAsync(Guid applicationId, string name, TimeSpan? releaseLifetime,
        TimeSpan? latestReleaseLifetime, bool useDefaultReleaseLifetime)
    {
        var branch = new Branch
        {
            Id = Guid.NewGuid(),
            ApplicationId = applicationId,
            Name = name,
            CreatedAt = DateTime.UtcNow,
            ReleaseLifetime = releaseLifetime,
            LatestReleaseLifetime = latestReleaseLifetime,
            UseDefaultReleaseLifetime = useDefaultReleaseLifetime
        };
        await branchRepository.CreateBranchAsync(branch);
        return branch;
    }

    public Task UpdateBranchAsync(Guid branchId, TimeSpan? releaseLifetime,
        TimeSpan? latestReleaseLifetime, bool useDefaultReleaseLifetime)
    {
        return branchRepository.UpdateBranchAsync(branchId, releaseLifetime, latestReleaseLifetime, useDefaultReleaseLifetime);
    }

    public Task DeleteBranchAsync(Guid branchId)
    {
        return branchRepository.DeleteBranchAsync(branchId);
    }

    public async Task<Branch> GetOrCreateBranchAsync(Guid applicationId, string name)
    {
        return await GetBranchByNameAsync(applicationId, name) ??
               await CreateBranchAsync(applicationId, name, null, null, true);
    }
}