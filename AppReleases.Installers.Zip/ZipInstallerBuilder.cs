using System.IO.Compression;
using System.Text.Json.Nodes;
using AppReleases.Core.Abstractions;
using AppReleases.Models;

namespace AppReleases.Installers.Zip;

public class ZipInstallerBuilder(IFileRepository fileRepository) : IInstallerBuilder
{
    public string Key => "Zip";

    public string DisplayName => "Архив ZIP";

    public string Description => "Просто собирает все ассеты в zip.";

    public async Task<BuiltInstaller> Build(Application application, Release release, IEnumerable<Asset> assets,
        JsonObject settings, CancellationToken cancellationToken = default)
    {
        var zipStream = new MemoryStream();

        using (var zip = new ZipArchive(zipStream, ZipArchiveMode.Create))
        {
            foreach (var asset in assets)
            {
                await using var zipEntry = zip.CreateEntry(asset.FileName).Open();
                await using var stream =
                    await fileRepository.DownloadFileAsync(FileRepositoryBucket.Assets, asset.FileId);
                await stream.CopyToAsync(zipEntry, cancellationToken);
            }
        }

        return new BuiltInstaller
        {
            FileStream = new MemoryStream(zipStream.ToArray()),
            FileName = $"{application.Name}_{release.Version}.zip"
        };
    }
}