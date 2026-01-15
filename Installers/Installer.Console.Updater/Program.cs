using Installer.Shared;

if (await PlatformsHelper.EnsureAdminRights())
    return 0;
try
{
    var installer = new Installer.Shared.Installer();
    if (Environment.GetCommandLineArgs().Contains("--update"))
        await installer.UpdateRelease();
    else
        await installer.UninstallRelease();
}
catch (Exception e)
{
    Console.WriteLine(e);
    Console.WriteLine("Нажмите любую клавишу для завершения");
    Console.ReadKey();
    return 1;
}

return 0;