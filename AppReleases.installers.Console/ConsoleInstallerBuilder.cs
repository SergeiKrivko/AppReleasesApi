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
        var filename = $"wwwroot/static/installers/Installer.Console_{platform}.exe";
        return await File.ReadAllBytesAsync(Path.Join(AppContext.BaseDirectory, filename), token);
    }

    public async Task<BuiltInstaller> Build(InstallerBuilderContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context.ApiUrl);
        var installerPlatform = context.Settings["installerPlatform"]?.GetValue<string>();
        ArgumentNullException.ThrowIfNull(installerPlatform);
        var bytes = await ReadInstallerAsync(installerPlatform, cancellationToken);
        bytes = bytes
            .PatchString("API_URL_HERE____________________________________________", context.ApiUrl)
            .PatchString("APPLICATION_ID_HERE_____________________", context.Application.Id.ToString())
            .PatchString("RELEASE_ID_HERE_________________________", context.Release.Id.ToString());
        var filename = $"{context.Application.Key}_{context.Release.Version}";
        if (installerPlatform.StartsWith("win"))
            filename += ".exe";
        return new BuiltInstaller
        {
            FileName = filename,
            FileStream = new MemoryStream(bytes),
        };
    }
}