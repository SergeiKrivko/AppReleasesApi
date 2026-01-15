using System.IO.Compression;

namespace ConsoleInstaller;

public class Installer
{
    private readonly string _appDirectory;
    private readonly ApiClient _apiClient;

    public Installer(string appDirectory)
    {
        _appDirectory = appDirectory;
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
        var url = await _apiClient.GetAssetsUrlAsync(Config.ReleaseId.TrimEnd('\0'));
        var tempDir = await DownloadAssets(url, _appDirectory);
    }
}