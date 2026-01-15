using AppReleases.Core.Abstractions;
using AppReleases.installers.Console;
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
        new ZipInstallerBuilder(fileRepository),
        new ConsoleInstallerBuilder(),
    }.ToDictionary(x => x.Key, x => x);

    private static TimeSpan DefaultUrlLifetime { get; } = TimeSpan.FromHours(1);

    public Task<IEnumerable<IInstallerBuilder>> GetAllBuildersAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IEnumerable<IInstallerBuilder>>(_builders.Values);
    }

    public async Task<string> BuildInstallerAsync(Models.Application application, Release release, Guid builderId,
        string apiUrl,
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
        {
            await builtInstallerRepository.UpdateDownloadTimeAsync(builderUsage.Id, cancellationToken);
            return await fileRepository.GetDownloadUrlAsync(FileRepositoryBucket.Installers, existing.FileId,
                existing.FileName, DefaultUrlLifetime);
        }

        var assets = await assetRepository.GetAllAssetsAsync(release.Id);
        var builtInstaller =
            await builder.Build(new InstallerBuilderContext
            {
                Application = application,
                Release = release,
                Settings = builderUsage.Settings,
                Assets = assets,
                ApiUrl = apiUrl
            }, cancellationToken);
        var id = Guid.NewGuid();
        await fileRepository.UploadFileAsync(FileRepositoryBucket.Installers, id, builtInstaller.FileName,
            builtInstaller.FileStream);
        await builtInstallerRepository.CreateBuiltInstallerAsync(release.Id, builderUsage.Id, id,
            builtInstaller.FileName,
            cancellationToken);
        return await fileRepository.GetDownloadUrlAsync(FileRepositoryBucket.Installers, id,
            builtInstaller.FileName, DefaultUrlLifetime);
    }

    public async Task<Guid> AddNewInstallerBuilderForApplicationAsync(string builderKey, string? name,
        Guid applicationId,
        TimeSpan installerLifetime, string[] platforms, CancellationToken cancellationToken = default)
    {
        if (!_builders.ContainsKey(builderKey))
            throw new KeyNotFoundException();
        return await installerBuilderRepository.CreateInstallerBuilderForApplicationAsync(applicationId, builderKey,
            name,
            installerLifetime, platforms, cancellationToken);
    }

    public async Task RemoveInstallerBuilderAsync(Guid builderId, CancellationToken cancellationToken = default)
    {
        await installerBuilderRepository.DeleteInstallerBuilderAsync(builderId, cancellationToken);
    }

    public async Task UpdateInstallerBuilderAsync(Guid builderId, string? name, TimeSpan installerLifetime,
        string[] platforms,
        CancellationToken cancellationToken = default)
    {
        await installerBuilderRepository.UpdateInstallerBuilderAsync(builderId, name, installerLifetime, platforms,
            cancellationToken);
    }

    public Task<IEnumerable<InstallerBuilderUsage>> GetAllInstallerBuildersOfApplicationAsync(Guid applicationId,
        CancellationToken cancellationToken = default)
    {
        return installerBuilderRepository.GetAllInstallerBuildersOfApplicationAsync(applicationId, cancellationToken);
    }
}