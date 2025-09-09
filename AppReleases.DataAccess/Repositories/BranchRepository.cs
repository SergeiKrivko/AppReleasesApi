using AppReleases.Core.Abstractions;
using AppReleases.Models;
using AppReleases.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppReleases.DataAccess.Repositories;

public class BranchRepository(AppReleasesDbContext dbContext) : IBranchRepository
{
    public async Task<IEnumerable<Branch>> GetAllBranchesAsync(Guid applicationId)
    {
        var entities = await dbContext.Branches
            .Where(x => x.ApplicationId == applicationId)
            .Where(x => x.DeletedAt == null)
            .ToArrayAsync();
        return entities.Select(BranchFromEntity);
    }

    public async Task<Branch> GetBranchByIdAsync(Guid branchId)
    {
        var entity = await dbContext.Branches
            .Where(x => x.BranchId == branchId)
            .SingleAsync();
        return BranchFromEntity(entity);
    }

    public async Task<Branch?> GetBranchByNameAsync(Guid applicationId, string name)
    {
        var entity = await dbContext.Branches
            .Where(x => x.ApplicationId == applicationId)
            .Where(x => x.Name == name)
            .Where(x => x.DeletedAt == null)
            .SingleOrDefaultAsync();
        return entity is null ? null : BranchFromEntity(entity);
    }

    public async Task CreateBranchAsync(Branch branch)
    {
        var entity = EntityFromBranch(branch);
        await dbContext.Branches.AddAsync(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteBranchAsync(Guid branchId)
    {
        await dbContext.Branches
            .Where(x => x.BranchId == branchId)
            .ExecuteUpdateAsync(x => x
                .SetProperty(e => e.DeletedAt, DateTime.UtcNow)
            );
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateBranchAsync(Guid branchId, TimeSpan? releaseLifetime, TimeSpan? latestReleaseLifetime,
        bool useDefaultReleaseLifetime = true)
    {
        await dbContext.Branches
            .Where(x => x.BranchId == branchId)
            .ExecuteUpdateAsync(x => x
                .SetProperty(e => e.ReleaseLifetime, releaseLifetime)
                .SetProperty(e => e.LatestReleaseLifetime, latestReleaseLifetime)
                .SetProperty(e => e.UseDefaultReleaseLifetime, useDefaultReleaseLifetime)
            );
        await dbContext.SaveChangesAsync();
    }

    private static Branch BranchFromEntity(BranchEntity entity)
    {
        return new Branch
        {
            Id = entity.BranchId,
            ApplicationId = entity.ApplicationId,
            Name = entity.Name,
            CreatedAt = entity.CreatedAt,
            DeletedAt = entity.DeletedAt,
            ReleaseLifetime = entity.ReleaseLifetime,
            LatestReleaseLifetime = entity.LatestReleaseLifetime,
            UseDefaultReleaseLifetime = entity.UseDefaultReleaseLifetime,
        };
    }

    private static BranchEntity EntityFromBranch(Branch branch)
    {
        return new BranchEntity()
        {
            BranchId = branch.Id,
            ApplicationId = branch.ApplicationId,
            Name = branch.Name,
            CreatedAt = branch.CreatedAt,
            DeletedAt = branch.DeletedAt,
            ReleaseLifetime = branch.ReleaseLifetime,
            LatestReleaseLifetime = branch.LatestReleaseLifetime,
            UseDefaultReleaseLifetime = branch.UseDefaultReleaseLifetime,
        };
    }
}