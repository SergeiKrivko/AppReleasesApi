using ConsoleInstaller;

if (await PlatformsHelper.EnsureAdminRights())
    return 0;
var installer = new Installer();
await installer.InstallRelease();

return 0;