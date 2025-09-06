using AppReleases.Core.Abstractions;
using AppReleases.Core.Models;

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

    public Task<Branch> GetBranchByNameAsync(Guid applicationId, string name)
    {
        return branchRepository.GetBranchByNameAsync(applicationId, name);
    }

    public async Task<Branch> CreateBranchAsync(Guid applicationId, string name)
    {
        var branch = new Branch
        {
            Id = Guid.NewGuid(),
            ApplicationId = applicationId,
            Name = name,
            CreatedAt = DateTime.Now,
            Duration = null,
        };
        await branchRepository.CreateBranchAsync(branch);
        return branch;
    }

    public Task UpdateBranchAsync(Guid branchId, TimeSpan? duration)
    {
        return branchRepository.UpdateBranchAsync(branchId, duration);
    }

    public Task DeleteBranchAsync(Guid branchId)
    {
        return branchRepository.DeleteBranchAsync(branchId);
    }
}