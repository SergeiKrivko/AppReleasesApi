using AppReleases.Core.Abstractions;
using AppReleases.Models;

namespace AppReleases.installers.Console;

public class ConsoleInstallerBuilder : IInstallerBuilder
{
    public string Key => "Console";
    public string DisplayName => "Консольный установщик";

    public string Description =>
        "Установщик с консольным интерфейсом, который скачивает и устанавливает приложение в нужное место";

    private static async Task<byte[]> ReadInstallerAsync(string platform, CancellationToken token)
    {
        var filename = $"wwwroot/static/installers/ConsoleInstaller_{platform}";
        if (platform.StartsWith("win"))
            filename += ".exe";
        return await File.ReadAllBytesAsync(Path.Join(AppContext.BaseDirectory, filename), token);
    }

    public async Task<BuiltInstaller> Build(InstallerBuilderContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context.ApiUrl);
        if (context.Release.Platform == null)
            throw new NotSupportedException("Installer for multiplatform release is not supported");
        var bytes = await ReadInstallerAsync(context.Release.Platform, cancellationToken);
        bytes = bytes
            .PatchString("API_URL_HERE____________________________________________", context.ApiUrl)
            .PatchString("APPLICATION_ID_HERE_____________________", context.Application.Id.ToString())
            .PatchString("RELEASE_ID_HERE_________________________", context.Release.Id.ToString());
        return new BuiltInstaller
        {
            FileName = $"{context.Application.Key}_{context.Release.Version}.exe",
            FileStream = new MemoryStream(bytes),
        };
    }
}