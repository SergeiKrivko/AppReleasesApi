using Installer.Console;
using Installer.Shared;
using Installer.Shared.Schemas;

if (await PlatformsHelper.EnsureAdminRights())
    return 0;
var installer = new Installer.Shared.Installer()
    .WithConfig(new ConfigSchema
    {
        ApiUrl = Config.ApiUrl.TrimEnd('\0'),
        ApplicationId = Guid.Parse(Config.ApplicationId.TrimEnd('\0')),
        ReleaseId = Guid.Parse(Config.ReleaseId.TrimEnd('\0')),
    });
await installer.InstallRelease();

Console.WriteLine("Нажмите любую клавишу для завершения");
Console.ReadKey();

return 0;