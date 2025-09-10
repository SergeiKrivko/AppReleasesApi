using AppReleases.Core.Abstractions;
using AppReleases.DataAccess.Entities;
using AppReleases.Models;
using Microsoft.EntityFrameworkCore;

namespace AppReleases.DataAccess.Repositories;

public class InstallerRepository(AppReleasesDbContext dbContext) : IInstallerRepository
{
    public async Task<Installer> GetInstallerByIdAsync(Guid id)
    {
        var result = await dbContext.Installers
            .Where(x => x.InstallerId == id)
            .FirstAsync();
        return InstallerFromEntity(result);
    }

    public async Task<IEnumerable<Installer>> GetAllInstallersAsync(Guid releaseId)
    {
        var result = await dbContext.Installers
            .Where(x => x.ReleaseId == releaseId)
            .ToListAsync();
        return result.Select(InstallerFromEntity);
    }

    public async Task<Installer?> FindInstallerAsync(Guid releaseId, string fileName)
    {
        var result = await dbContext.Installers
            .Where(x => x.ReleaseId == releaseId && x.FileName == fileName)
            .SingleOrDefaultAsync();
        return result is null ? null : InstallerFromEntity(result);
    }

    public async Task CreateInstallerAsync(Installer installer)
    {
        var entity = InstallerToEntity(installer);
        await dbContext.Installers.AddAsync(entity);
    }

    public async Task DeleteInstallerAsync(Guid installerId)
    {
        await dbContext.Installers
            .Where(x => x.InstallerId == installerId)
            .ExecuteUpdateAsync(x => x.SetProperty(e => e.DeletedAt, DateTime.UtcNow));
    }

    private static Installer InstallerFromEntity(InstallerEntity entity)
    {
        return new Installer
        {
            InstallerId = entity.InstallerId,
            ReleaseId = entity.ReleaseId,
            FileName = entity.FileName,
            CreatedAt = entity.CreatedAt,
            DeletedAt = entity.DeletedAt,
            FileId = entity.FileId,
        };
    }

    private static InstallerEntity InstallerToEntity(Installer installer)
    {
        return new InstallerEntity
        {
            InstallerId = installer.InstallerId,
            ReleaseId = installer.ReleaseId,
            FileName = installer.FileName,
            CreatedAt = installer.CreatedAt,
            DeletedAt = installer.DeletedAt,
            FileId = installer.FileId,
        };
    }
}