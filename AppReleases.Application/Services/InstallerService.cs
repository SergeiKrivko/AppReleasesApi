using AppReleases.Core.Abstractions;
using AppReleases.Models;

namespace AppReleases.Application.Services;

public class InstallerService(
    IInstallerRepository installerRepository,
    IFileRepository fileRepository) : IInstallerService
{
    public Task<Installer> GetInstallerByIdAsync(Guid id)
    {
        return installerRepository.GetInstallerByIdAsync(id);
    }

    public Task<IEnumerable<Installer>> GetAllInstallersAsync(Guid releaseId)
    {
        return installerRepository.GetAllInstallersAsync(releaseId);
    }

    public Task<Installer?> FindInstallerAsync(Guid releaseId, string fileName)
    {
        return installerRepository.FindInstallerAsync(releaseId, fileName);
    }

    public async Task<Installer> CreateInstallerAsync(Guid releaseId, string fileName, Stream stream)
    {
        var installer = new Installer
        {
            InstallerId = Guid.NewGuid(),
            ReleaseId = releaseId,
            FileId = Guid.NewGuid(),
            FileName = fileName,
            CreatedAt = DateTime.UtcNow,
        };
        await installerRepository.CreateInstallerAsync(installer);
        await fileRepository.UploadFileAsync(FileRepositoryBucket.Installers, installer.FileId,
            Path.GetExtension(fileName), stream);
        return installer;
    }

    public async Task DeleteInstallerAsync(Guid installerId)
    {
        var installer = await installerRepository.GetInstallerByIdAsync(installerId);
        await fileRepository.DeleteFileAsync(FileRepositoryBucket.Installers, installer.FileId,
            Path.GetExtension(installer.FileName));
        await installerRepository.DeleteInstallerAsync(installerId);
    }

    public async Task<string> GetDownloadUrlAsync(Guid installerId)
    {
        var installer = await installerRepository.GetInstallerByIdAsync(installerId);
        return await fileRepository.GetDownloadUrlAsync(FileRepositoryBucket.Installers, installerId,
            Path.GetExtension(installer.FileName), TimeSpan.FromHours(1));
    }
}