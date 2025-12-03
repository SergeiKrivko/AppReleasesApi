using AppReleases.Models;

namespace AppReleases.Core.Abstractions;

public interface IBuiltInstallerRepository
{
    public Task<BuiltInstallerModel?> FindExistingInstallerAsync(Guid releaseId, Guid builderId,
        CancellationToken cancellationToken = default);

    public Task<Guid> CreateBuiltInstallerAsync(Guid releaseId, Guid builderId, Guid fileId,
        string fileName, CancellationToken cancellationToken = default);
}