using AppReleases.Models;

namespace AppReleases.Core.Abstractions;

public interface IInstallerService
{
    public Task<string> BuildInstallerAsync(Application application, Release release, Guid builderId,
        CancellationToken cancellationToken = default);

    public Task<Guid> AddNewInstallerBuilderForApplicationAsync(string builderKey, Guid applicationId,
        TimeSpan installerLifetime, CancellationToken cancellationToken = default);

    public Task<IEnumerable<InstallerBuilderUsage>> GetAllInstallerBuildersOfApplicationAsync(Guid applicationId,
        CancellationToken cancellationToken = default);
}