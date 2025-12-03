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

    public async Task<string> BuildInstallerAsync(string builderKey, Models.Application application, Release release,
        CancellationToken cancelationToken = default)
    {
        var builder = _builders[builderKey];
        var applicationBuilders =
            await installerBuilderRepository.GetInstallerBuildersOfApplicationAsync(application.Id);
        var builderUsage = applicationBuilders.FirstOrDefault(x => x.BuilderKey == builderKey);
        if (builderUsage == null)
            throw new KeyNotFoundException();
        var existing =
            await builtInstallerRepository.FindExistingInstallerAsync(release.Id, builderUsage.Id, cancelationToken);
        if (existing != null)
            return await fileRepository.GetDownloadUrlAsync(FileRepositoryBucket.Installers, existing.FileId,
                existing.FileName, TimeSpan.FromSeconds(2));

        var assets = await assetRepository.GetAllAssetsAsync(release.Id);
        var builtInstaller = await builder.Build(application, release, assets, builderUsage.Settings, cancelationToken);
        var id = Guid.NewGuid();
        await fileRepository.UploadFileAsync(FileRepositoryBucket.Installers, id, builtInstaller.FileName,
            builtInstaller.FileStream);
        await builtInstallerRepository.CreateBuiltInstallerAsync(release.Id, builderUsage.Id, id, builtInstaller.FileName,
            cancelationToken);
        return await fileRepository.GetDownloadUrlAsync(FileRepositoryBucket.Installers, id,
            builtInstaller.FileName, TimeSpan.FromSeconds(2));
    }
}