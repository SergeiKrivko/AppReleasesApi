using Installer.Shared;

if (await PlatformsHelper.EnsureAdminRights())
    return 0;
var installer = new Installer.Shared.Installer();
if (Environment.GetCommandLineArgs().Contains("--update"))
    await installer.UpdateRelease();
else
    await installer.UninstallRelease();
await installer.InstallRelease();

Console.WriteLine("Нажмите любую клавишу для завершения");
Console.ReadKey();

return 0;