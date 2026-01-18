using System.IO.Compression;
using System.Text;
using System.Text.Json.Nodes;
using AppReleases.Core.Abstractions;
using AppReleases.Installers.Shared;
using AppReleases.Models;

namespace AppReleases.Installers.Zip;

public class ZipInstallerBuilder(IFileRepository fileRepository) : IInstallerBuilder
{
    public string Key => "Zip";

    public string DisplayName => "Архив ZIP";

    public string Description => "Просто собирает все ассеты в zip.";

    public async Task<BuiltInstaller> Build(InstallerBuilderContext context,
        CancellationToken cancellationToken = default)
    {
        var zipStream = new MemoryStream();

        using (var zip = new ZipArchive(zipStream, ZipArchiveMode.Create))
        {
            foreach (var asset in context.Assets ?? [])
            {
                await using var zipEntry = zip.CreateEntry(asset.FileName).Open();
                await using var stream =
                    await fileRepository.DownloadFileAsync(FileRepositoryBucket.Assets, asset.FileId);
                await stream.CopyToAsync(zipEntry, cancellationToken);
            }

            var updaterPlatform = context.Settings["updaterPlatform"]?.GetValue<string?>();
            if (updaterPlatform != null)
            {
                var updaterBytes =
                    await InstallersHelper.ReadStaticAsync($"Installer.Console.Updater_{context.Release.Platform}.exe",
                        cancellationToken);
                await using (var zipEntry =
                             zip.CreateEntry(updaterPlatform.StartsWith("win") ? "Uninstall.exe" : "Uninstall").Open())
                {
                    await zipEntry.WriteAsync(updaterBytes, cancellationToken);
                }

                await using (var zipEntry = zip.CreateEntry("InstallerConfig.json").Open())
                {
                    var helper = new InstallersHelper(context);
                    await zipEntry.WriteAsync(Encoding.UTF8.GetBytes(helper.GenerateConfig()), cancellationToken);
                }
            }
        }

        return new BuiltInstaller
        {
            FileStream = new MemoryStream(zipStream.ToArray()),
            FileName = $"{context.Application.Key}_{context.Release.Version}.zip"
        };
    }
}