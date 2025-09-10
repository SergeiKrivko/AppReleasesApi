using AppReleases.Models;

namespace AppReleases.Core.Abstractions;

public interface IInstallerRepository
{
    public Task<Installer> GetInstallerByIdAsync(Guid id);
    public Task<IEnumerable<Installer>> GetAllInstallersAsync(Guid releaseId);
    public Task<Installer?> FindInstallerAsync(Guid releaseId, string fileName);
    public Task CreateInstallerAsync(Installer installer);
    public Task DeleteInstallerAsync(Guid installerId);
}