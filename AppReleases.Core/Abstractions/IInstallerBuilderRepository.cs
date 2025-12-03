using AppReleases.Models;

namespace AppReleases.Core.Abstractions;

public interface IInstallerBuilderRepository
{
    public Task<IEnumerable<InstallerBuilderUsage>> GetInstallerBuildersOfApplicationAsync(Guid applicationId);
}