using System.IO.Compression;
using System.Text.Json.Nodes;
using AppReleases.Core.Abstractions;
using AppReleases.Models;

namespace AppReleases.Installers.Zip;

public class ZipInstallerBuilder(IFileRepository fileRepository) : IInstallerBuilder
{
    public string Key => "Zip";

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

        zipStream.Seek(0, SeekOrigin.Begin);
        return new BuiltInstaller
        {
            FileStream = zipStream,
            FileName = $"{application.Name}_{release.Version}.zip"
        };
    }
}