﻿using AppReleases.Core.Models;

namespace AppReleases.Core.Abstractions;

public interface IBranchRepository
{
    public Task<IEnumerable<Branch>> GetAllBranchesAsync(Guid applicationId);
    public Task<Branch> GetBranchByIdAsync(Guid branchId);
    public Task<Branch> GetBranchByNameAsync(Guid applicationId, string name);
    public Task CreateBranchAsync(Branch branch);
    public Task DeleteBranchAsync(Guid branchId);
    public Task MarkBranchAsMergedAsync(Guid branchId);
    public Task DeleteBranchAsMergedAsync(Guid branchId);
}