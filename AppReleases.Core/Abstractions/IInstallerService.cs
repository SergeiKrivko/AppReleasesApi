using AppReleases.Models;

namespace AppReleases.Core.Abstractions;

public interface IInstallerService
{
    public Task<IEnumerable<IInstallerBuilder>> GetAllBuildersAsync(CancellationToken cancellationToken);

    public Task<string> BuildInstallerAsync(Application application, Release release, Guid builderId,
        CancellationToken cancellationToken = default);

    public Task<Guid> AddNewInstallerBuilderForApplicationAsync(string builderKey, string? name, Guid applicationId,
        TimeSpan installerLifetime, string[] platforms, CancellationToken cancellationToken = default);

    public Task<IEnumerable<InstallerBuilderUsage>> GetAllInstallerBuildersOfApplicationAsync(Guid applicationId,
        CancellationToken cancellationToken = default);
}