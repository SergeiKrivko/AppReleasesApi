using AppReleases.Models;

namespace AppReleases.Core.Abstractions;

public interface IInstallerService
{
    public Task<Installer> GetInstallerByIdAsync(Guid id);
    public Task<IEnumerable<Installer>> GetAllInstallersAsync(Guid releaseId);
    public Task<Installer?> FindInstallerAsync(Guid releaseId, string fileName);
    public Task<Installer> CreateInstallerAsync(Guid releaseId, string fileName, Stream stream);
    public Task DeleteInstallerAsync(Guid installerId);
    public Task<string> GetDownloadUrlAsync(Guid installerId);
}