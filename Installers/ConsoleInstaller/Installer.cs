using System.IO.Compression;

namespace ConsoleInstaller;

public class Installer
{
    private readonly ApiClient _apiClient;

    public Installer()
    {
        _apiClient = new ApiClient();
    }

    private static async Task<string> DownloadAssets(string url)
    {
        var client = new HttpClient();
        await using var stream = await client.GetStreamAsync(url);
        var tempPath = Path.GetTempFileName();
        ZipFile.ExtractToDirectory(stream, tempPath);
        return tempPath;
    }

    private static async Task<string> DownloadAssets(string url, string directory)
    {
        var client = new HttpClient();
        await using var stream = await client.GetStreamAsync(url);
        ZipFile.ExtractToDirectory(stream, directory);
        return directory;
    }

    public async Task InstallRelease()
    {
        Console.WriteLine("Получение информации о приложении...");
        var application = await _apiClient.GetApplicationInfoAsync(Config.ApplicationId.TrimEnd('\0'));
        var directory = PlatformsHelper.GetInstallPath(application);
        Console.WriteLine("Подготовка к загрузке...");
        var url = await _apiClient.GetAssetsUrlAsync(Config.ReleaseId.TrimEnd('\0'));
        Console.WriteLine("Загрузка...");
        await DownloadAssets(url, directory);
        Console.WriteLine("Установка завершена!");
    }
}