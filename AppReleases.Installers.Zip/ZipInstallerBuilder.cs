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

        var topLevelDirectory = context.Settings["topLevelDirectory"]?.GetValue<string?>();
        var pathPrefix = string.IsNullOrEmpty(topLevelDirectory) ? string.Empty : $"{topLevelDirectory}/";

        using (var zip = new ZipArchive(zipStream, ZipArchiveMode.Create))
        {
            foreach (var asset in context.Assets ?? [])
            {
                await using var zipEntry = zip.CreateEntry(pathPrefix + asset.FileName).Open();
                await using var stream =
                    await fileRepository.DownloadFileAsync(FileRepositoryBucket.Assets, asset.FileId);
                await stream.CopyToAsync(zipEntry, cancellationToken);
            }

            var updaterPlatform = context.Settings["updaterPlatform"]?.GetValue<string?>();
            var installerDirectory = context.Settings["installerDirectory"]?.GetValue<string?>();
            if (!string.IsNullOrEmpty(installerDirectory))
                pathPrefix += installerDirectory + "/";
            if (updaterPlatform?.Length > 4)
            {
                var updaterBytes =
                    await InstallersHelper.ReadStaticAsync($"Installer.Console.Updater_{updaterPlatform}.exe",
                        cancellationToken);
                await using (var zipEntry =
                             zip.CreateEntry(pathPrefix +
                                             (updaterPlatform.StartsWith("win") ? "Uninstall.exe" : "Uninstall"))
                                 .Open())
                {
                    await zipEntry.WriteAsync(updaterBytes, cancellationToken);
                }

                await using (var zipEntry = zip.CreateEntry($"{pathPrefix}InstallerConfig.json").Open())
                {
                    var helper = new InstallersHelper(context);
                    var configJson = helper.GenerateConfig(InstallationPathFromInstallerDirectory(installerDirectory));
                    await zipEntry.WriteAsync(Encoding.UTF8.GetBytes(configJson), cancellationToken);
                }
            }
        }

        return new BuiltInstaller
        {
            FileStream = new MemoryStream(zipStream.ToArray()),
            FileName = $"{context.Application.Key}_{context.Release.Version}.zip"
        };
    }

    private static string InstallationPathFromInstallerDirectory(string? installerDirectory)
    {
        if (string.IsNullOrEmpty(installerDirectory))
            return string.Empty;
        var count = installerDirectory.Split('/').Length;
        var builder = new StringBuilder();
        for (int i = 0; i < count; i++)
            builder.Append("../");
        return builder.ToString();
    }
}