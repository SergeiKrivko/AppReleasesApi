using Installer.Shared;

if (await PlatformsHelper.EnsureAdminRights())
    return 0;
var installer = new Installer.Shared.Installer();
if (Environment.GetCommandLineArgs().Contains("--update"))
    await installer.UpdateRelease();
else
    await installer.UninstallRelease();
await installer.InstallRelease();

return 0;