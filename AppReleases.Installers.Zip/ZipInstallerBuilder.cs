using System.IO.Compression;
using System.Text.Json.Nodes;
using AppReleases.Core.Abstractions;
using AppReleases.Models;

namespace AppReleases.Installers.Zip;

public class ZipInstallerBuilder(
    IFileRepository fileRepository,
    IAssetRepository assetRepository) : IInstallerBuilder
{
    public string Key => "Zip";

    public async Task<BuiltInstaller> Build(Application application, Release release, JsonObject settings)
    {
        var zipStream = new MemoryStream();
        var assets = await assetRepository.GetAllAssetsAsync(release.Id);

        using var zip = new ZipArchive(zipStream, ZipArchiveMode.Create);
        foreach (var asset in assets)
        {
            await using var zipEntry = zip.CreateEntry(asset.FileName).Open();
            await using var stream =
                await fileRepository.DownloadFileAsync(FileRepositoryBucket.Assets, asset.FileId);
            await stream.CopyToAsync(zipEntry);
        }

        return new BuiltInstaller
        {
            File = zipStream,
            FileName = $"{application.Name}_{release.Version}.zip"
        };
    }
}