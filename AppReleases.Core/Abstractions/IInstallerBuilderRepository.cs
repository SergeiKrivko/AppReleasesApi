using AppReleases.Models;

namespace AppReleases.Core.Abstractions;

public interface IInstallerBuilderRepository
{
    public Task<IEnumerable<InstallerBuilderUsage>> GetAllInstallerBuildersOfApplicationAsync(Guid applicationId, CancellationToken cancellationToken = default);

    public Task<InstallerBuilderUsage?> GetInstallerBuilderByIdAsync(Guid builderId,
        CancellationToken cancellationToken = default);

    public Task<Guid> CreateInstallerBuilderForApplicationAsync(Guid applicationId, string builderKey, string? name,
        TimeSpan installerLifetime, string[] platforms,
        CancellationToken cancellationToken = default);
}