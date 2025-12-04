using AppReleases.Core.Abstractions;
using AppReleases.Models;
using Microsoft.Extensions.Logging;

namespace AppReleases.Application.Services;

public class CleanerService(
    IReleaseService releaseService,
    IBranchService branchService,
    IApplicationService applicationService,
    IAssetRepository assetRepository,
    IInstallerBuilderRepository installerBuilderRepository,
    IBuiltInstallerRepository builtInstallerRepository,
    IFileRepository fileRepository,
    ILogger<CleanerService> logger) : ICleanerService
{
    public async Task<int> ClearOldReleasesAsync(CancellationToken cancellationToken)
    {
        var count = 0;
        var applications = await applicationService.GetAllApplicationsAsync();
        foreach (var application in applications)
        {
            count += await ClearOldReleasesOfApplicationAsync(application);
            cancellationToken.ThrowIfCancellationRequested();
        }

        return count;
    }

    public async Task<int> ClearOldReleasesOfApplicationAsync(Guid applicationId)
    {
        var application = await applicationService.GetApplicationByIdAsync(applicationId);
        return await ClearOldReleasesOfApplicationAsync(application);
    }

    public async Task<int> ClearOldReleasesOfApplicationAsync(Models.Application application)
    {
        var count = 0;
        var branches = await branchService.GetAllBranchesAsync(application.Id);
        foreach (var branch in branches)
        {
            count += await ClearOldReleasesOfBranchAsync(application, branch);
        }

        return count;
    }

    public async Task<int> ClearOldReleasesOfBranchAsync(Guid branchId)
    {
        var branch = await branchService.GetBranchByIdAsync(branchId);
        var application = await applicationService.GetApplicationByIdAsync(branch.ApplicationId);
        return await ClearOldReleasesOfBranchAsync(application, branch);
    }

    public async Task<int> ClearOldReleasesOfBranchAsync(Models.Application application, Branch branch)
    {
        var count = 0;
        var releases = (await releaseService.GetAllReleasesOfBranchAsync(branch.Id)).ToArray();
        foreach (var release in releases)
        {
            var isLatest = releases
                .FirstOrDefault(r => r.Platform == release.Platform && r.Version > release.Version) == null;
            var lifetime = isLatest
                ? branch.UseDefaultReleaseLifetime
                    ? application.DefaultLatestReleaseLifetime
                    : branch.LatestReleaseLifetime
                : branch.UseDefaultReleaseLifetime
                    ? application.DefaultReleaseLifetime
                    : branch.ReleaseLifetime;
            if (lifetime != null && release.CreatedAt + lifetime < DateTime.UtcNow)
            {
                logger.LogInformation(
                    "Удаление релиза '{app}' - {version} - {platform}. Срок жизни закончился {expiredAt}",
                    application.Name, release.Version, release.Id, release.CreatedAt + lifetime
                );
                await releaseService.DeleteReleaseAsync(release.Id);
                count++;
            }
        }

        return count;
    }

    public async Task<int> ClearUnusedAssetsAsync(CancellationToken cancellationToken)
    {
        var count = 0;
        foreach (var asset in await assetRepository.GetUnusedAssetsAsync())
        {
            cancellationToken.ThrowIfCancellationRequested();
            logger.LogInformation("Удаление неиспользуемого ассета {name} -- {hash}", asset.FileName, asset.FileHash);
            await fileRepository.DeleteFileAsync(FileRepositoryBucket.Assets, asset.FileId);
            await assetRepository.DeleteAssetAsync(asset.Id);
            count++;
        }

        return count;
    }

    public async Task<int> ClearOldInstallersAsync(CancellationToken cancellationToken)
    {
        var count = 0;
        var applications = await applicationService.GetAllApplicationsAsync();
        foreach (var application in applications)
        {
            count += await ClearOldInstallersOfApplicationAsync(application.Id, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
        }

        return count;
    }

    public async Task<int> ClearOldInstallersOfApplicationAsync(Guid applicationId, CancellationToken cancellationToken)
    {
        var count = 0;
        foreach (var builder in await installerBuilderRepository
                     .GetAllInstallerBuildersOfApplicationAsync(applicationId, cancellationToken))
        {
            var lifetime = builder.InstallerLifetime ?? TimeSpan.FromDays(1);
            count += await builtInstallerRepository.DeleteInstallersDownloadedBeforeAsync(DateTime.UtcNow - lifetime,
                builder.Id,
                cancellationToken);
        }
        return count;
    }
}