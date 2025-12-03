using AppReleases.Core.Abstractions;
using AppReleases.DataAccess.Entities;
using AppReleases.Models;
using Microsoft.EntityFrameworkCore;

namespace AppReleases.DataAccess.Repositories;

public class BuiltInstallerRepository(AppReleasesDbContext dbContext) : IBuiltInstallerRepository
{
    public async Task<BuiltInstallerModel?> FindExistingInstallerAsync(Guid releaseId, Guid builderId,
        CancellationToken cancellationToken)
    {
        var entity = await dbContext.BuiltInstallers
            .Where(x => x.BuilderId == builderId && x.ReleaseId == releaseId && x.DeletedAt == null)
            .FirstOrDefaultAsync(cancellationToken);
        return entity is null ? null : BuiltInstallerFromEntity(entity);
    }

    public async Task UpdateDownloadTimeAsync(Guid builderId, CancellationToken cancellationToken = default)
    {
        await dbContext.BuiltInstallers
            .Where(x => x.BuilderId == builderId && x.DeletedAt == null)
            .ExecuteUpdateAsync(x => x.SetProperty(e => e.DownloadedAt, DateTime.UtcNow), cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> DeleteInstallersDownloadedBeforeAsync(DateTime time, Guid builderId,
        CancellationToken cancellationToken = default)
    {
        var result = await dbContext.BuiltInstallers
            .Where(x => x.BuilderId == builderId && x.DeletedAt == null && x.DownloadedAt < time)
            .ExecuteUpdateAsync(x => x.SetProperty(e => e.DeletedAt, DateTime.UtcNow), cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return result;
    }

    public async Task<Guid> CreateBuiltInstallerAsync(Guid releaseId, Guid builderId, Guid fileId, string fileName,
        CancellationToken cancellationToken = default)
    {
        var id = Guid.NewGuid();
        var entity = new BuiltInstallerEntity
        {
            BuilderId = builderId,
            ReleaseId = releaseId,
            FileId = fileId,
            FileName = fileName,
            CreatedAt = DateTime.UtcNow,
            DownloadedAt = DateTime.UtcNow,
            DeletedAt = null,
            Id = id,
        };
        await dbContext.BuiltInstallers.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return id;
    }

    private static BuiltInstallerModel BuiltInstallerFromEntity(BuiltInstallerEntity entity)
    {
        return new BuiltInstallerModel
        {
            FileId = entity.FileId,
            CreatedAt = entity.CreatedAt,
            DeletedAt = entity.DeletedAt,
            FileName = entity.FileName,
        };
    }
}