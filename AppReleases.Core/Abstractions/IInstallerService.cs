using AppReleases.Models;

namespace AppReleases.Core.Abstractions;

public interface IInstallerService
{
    public Task<string> BuildInstallerAsync(string builderKey, Application application, Release release,
        CancellationToken cancellationToken = default);
}