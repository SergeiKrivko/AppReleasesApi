using AppReleases.Core.Abstractions;
using AppReleases.Installers.Zip;
using AppReleases.Models;

namespace AppReleases.Application.Services;

public class InstallerService(
    IBuiltInstallerRepository builtInstallerRepository,
    IInstallerBuilderRepository installerBuilderRepository,
    IAssetRepository assetRepository,
    IFileRepository fileRepository) : IInstallerService
{
    private readonly Dictionary<string, IInstallerBuilder> _builders = new List<IInstallerBuilder>
    {
        new ZipInstallerBuilder(fileRepository)
    }.ToDictionary(x => x.Key, x => x);

    private static TimeSpan DefaultUrlLifetime { get; } = TimeSpan.FromHours(1);

    public async Task<string> BuildInstallerAsync(Models.Application application, Release release, Guid builderId,
        CancellationToken cancellationToken = default)
    {
        var builderUsage = await installerBuilderRepository.GetInstallerBuilderByIdAsync(builderId, cancellationToken);
        if (builderUsage == null)
            throw new ApplicationException($"No builder found with id {builderId}");
        var builder = _builders[builderUsage.BuilderKey];
        if (builderUsage == null)
            throw new KeyNotFoundException();
        var existing =
            await builtInstallerRepository.FindExistingInstallerAsync(release.Id, builderUsage.Id, cancellationToken);
        if (existing != null)
            return await fileRepository.GetDownloadUrlAsync(FileRepositoryBucket.Installers, existing.FileId,
                existing.FileName, DefaultUrlLifetime);

        var assets = await assetRepository.GetAllAssetsAsync(release.Id);
        var builtInstaller =
            await builder.Build(application, release, assets, builderUsage.Settings, cancellationToken);
        var id = Guid.NewGuid();
        await fileRepository.UploadFileAsync(FileRepositoryBucket.Installers, id, builtInstaller.FileName,
            builtInstaller.FileStream);
        await builtInstallerRepository.CreateBuiltInstallerAsync(release.Id, builderUsage.Id, id,
            builtInstaller.FileName,
            cancellationToken);
        return await fileRepository.GetDownloadUrlAsync(FileRepositoryBucket.Installers, id,
            builtInstaller.FileName, DefaultUrlLifetime);
    }

    public async Task<Guid> AddNewInstallerBuilderForApplicationAsync(string builderKey, Guid applicationId,
        TimeSpan installerLifetime, CancellationToken cancellationToken = default)
    {
        if (!_builders.ContainsKey(builderKey))
            throw new KeyNotFoundException();
        return await installerBuilderRepository.CreateInstallerBuilderForApplicationAsync(applicationId, builderKey,
            installerLifetime, cancellationToken);
    }

    public Task<IEnumerable<InstallerBuilderUsage>> GetAllInstallerBuildersOfApplicationAsync(Guid applicationId,
        CancellationToken cancellationToken = default)
    {
        return installerBuilderRepository.GetAllInstallerBuildersOfApplicationAsync(applicationId, cancellationToken);
    }
}