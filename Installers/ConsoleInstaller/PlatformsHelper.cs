using System.Diagnostics;
using System.Runtime.Versioning;
using System.Security.Principal;
using ConsoleInstaller.Schemas;

namespace ConsoleInstaller;

public static class PlatformsHelper
{
    public static string GetInstallPath(ApplicationSchema application)
    {
        if (OperatingSystem.IsWindows())
            return Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), application.Key);
        if (OperatingSystem.IsLinux())
            return Path.Join("/opt", application.Key);
        if (OperatingSystem.IsMacOS())
            return Path.Join("/Applications", application.Name + ".app");
        throw new PlatformNotSupportedException();
    }

    /// <summary>
    /// Проверяет права администратора и перезапускает приложение при необходимости
    /// </summary>
    /// <returns>True если приложение было перезапущено, иначе False</returns>
    public static bool EnsureAdminRights()
    {
        var args = Environment.GetCommandLineArgs();
        if (OperatingSystem.IsWindows())
        {
            if (CheckWindowsAdmin())
                return false;
            RelaunchAsWindowsAdministrator(args);
            return true;
        }
        else if (OperatingSystem.IsLinux())
        {
            if (CheckUnixRoot())
                return false;
            RelaunchAsUnixRoot(args);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Проверяет, запущено ли приложение с правами администратора
    /// </summary>
    [SupportedOSPlatform("windows")]
    private static bool CheckWindowsAdmin()
    {
        try
        {
            using var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        catch
        {
            // В случае ошибки считаем, что прав администратора нет
            return false;
        }
    }

    /// <summary>
    /// Проверка прав root в Unix-системах (Linux/macOS)
    /// </summary>
    [SupportedOSPlatform("linux")]
    private static bool CheckUnixRoot()
    {
        try
        {
            // В Unix-системах root имеет UID = 0
            return Environment.UserName == "root";
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Перезапускает приложение с правами администратора
    /// </summary>
    [SupportedOSPlatform("windows")]
    private static void RelaunchAsWindowsAdministrator(string[] args)
    {
        var exePath = args[0];
        var startInfo = new ProcessStartInfo
        {
            FileName = exePath,
            UseShellExecute = true,
            Verb = "runas", // Ключевой параметр для запуска от имени администратора
            Arguments = string.Join(" ", args.Skip(1))
        };

        Process.Start(startInfo);
    }

    /// <summary>
    /// Перезапуск с правами root в Unix-системах
    /// </summary>
    private static void RelaunchAsUnixRoot(string[] args)
    {
        var exePath = args[0];

        // Собираем команду для запуска с sudo
        var arguments = EscapeArguments(args.Skip(1).ToList());

        // Запускаем с sudo
        var startInfo = new ProcessStartInfo
        {
            FileName = "sudo",
            Arguments = $"{exePath} {arguments}",
            UseShellExecute = true
        };

        Process.Start(startInfo);
    }

    /// <summary>
    /// Экранирует аргументы командной строки
    /// </summary>
    private static string EscapeArguments(List<string> args)
    {
        if (args.Count == 0)
            return string.Empty;

        var escapedArgs = new string[args.Count];
        for (int i = 0; i < args.Count; i++)
        {
            // Экранируем кавычки и пробелы
            if (args[i].Contains(' ') || args[i].Contains('\"'))
                escapedArgs[i] = $"\"{args[i].Replace("\"", "\\\"")}\"";
            else
                escapedArgs[i] = args[i];
        }

        return string.Join(" ", escapedArgs);
    }
}