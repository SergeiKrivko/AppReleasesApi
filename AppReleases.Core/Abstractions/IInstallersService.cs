using AppReleases.Models;

namespace AppReleases.Core.Abstractions;

public interface IInstallersService
{
    public Task<string> BuildInstallerAsync(Application application, Release release);
}