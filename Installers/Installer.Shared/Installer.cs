using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using Installer.Shared.Schemas;

namespace Installer.Shared;

public class Installer
{
    private ConfigSchema? _config;
    private ApiClient? _apiClient;

    [MemberNotNull(nameof(_apiClient))]
    [MemberNotNull(nameof(_config))]
    private void ThrowIfNotInitialized()
    {
        if (_config is null || _apiClient is null)
            throw new Exception("Installer is not initialized");
    }

    public Installer WithConfig(ConfigSchema config)
    {
        _config = config;
        _apiClient = new ApiClient(config.ApiUrl);
        return this;
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

    private const string ConfigFileName = "InstallerConfig.json";
    private static string ConfigPath => Path.Join(AppContext.BaseDirectory, ConfigFileName);

    private async Task SaveConfig(string? path = null)
    {
        ThrowIfNotInitialized();
        await File.WriteAllTextAsync(path ?? ConfigPath,
            JsonSerializer.Serialize(_config, InstallerJsonSerializerContext.Default.ConfigSchema));
    }

    private async Task ReadConfig()
    {
        _config = JsonSerializer.Deserialize(await File.ReadAllTextAsync(ConfigPath),
            InstallerJsonSerializerContext.Default.ConfigSchema);
    }

    private void AddAssetsToConfig(IEnumerable<AssetSchema> assets)
    {
        ThrowIfNotInitialized();
        _config.Assets = _config.Assets
            .Concat(assets)
            .ToArray();
    }

    private void AddAssetsToConfig(string directory)
    {
        ThrowIfNotInitialized();
        AddAssetsToConfig(Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories)
            .Select(f => new AssetSchema
            {
                FileName = Path.GetRelativePath(directory, f),
                FileHash = BitConverter.ToString(SHA256.HashData(File.ReadAllBytes(f))).Replace("-", "")
            }));
    }

    public async Task InstallRelease()
    {
        ThrowIfNotInitialized();

        Console.WriteLine("Получение информации о приложении...");
        var application = await _apiClient.GetApplicationInfoAsync(_config.ApplicationId);
        var directory = PlatformsHelper.GetInstallPath(application);

        Console.WriteLine("Подготовка к загрузке...");
        var url = await _apiClient.GetAssetsUrlAsync(_config.ReleaseId);

        Console.WriteLine("Загрузка и установка...");

        await DownloadAssets(url, directory);

        Console.WriteLine("Завершение установки...");
        AddAssetsToConfig(directory);
        _config.InstalledReleaseId = _config.ReleaseId;
        _config.ReleaseId = Guid.Empty;
        await SaveConfig(Path.Join(directory, ConfigFileName));

        Console.WriteLine("Установка завершена!");
    }

    public async Task UninstallRelease()
    {
        await ReadConfig();
        ThrowIfNotInitialized();
        foreach (var asset in _config.Assets)
        {
            var path = Path.IsPathRooted(asset.FileName)
                ? asset.FileName
                : Path.Join(AppContext.BaseDirectory, asset.FileName);
            File.Delete(path);
        }

        File.Delete(ConfigPath);
    }

    public async Task UpdateRelease()
    {
        await ReadConfig();
        ThrowIfNotInitialized();

        Console.WriteLine("Получение информации о последнем релизе...");
        var latestRelease = await _apiClient.GetLatestReleaseAsync(_config.ApplicationId);
        if (latestRelease.Id == _config.InstalledReleaseId)
        {
            Console.WriteLine("Уже установлена последняя версия приложения. Обновление не требуется.");
            return;
        }

        Console.WriteLine("Подготовка к загрузке...");
        var pack = await _apiClient.GetAssetsPackAsync(latestRelease.Id, _config.Assets.Select(asset => new AssetSchema
        {
            FileName = asset.FileName,
            FileHash = BitConverter.ToString(SHA256.HashData(File.ReadAllBytes(asset.FileName))).Replace("-", "")
        }));

        Console.WriteLine("Загрузка обновления...");
        var tempPath = await DownloadAssets(pack.Url);

        Console.WriteLine("Установка обновления...");
        foreach (var path in pack.DeletedAssets)
            File.Delete(Path.IsPathRooted(path) ? path : Path.Join(AppContext.BaseDirectory, path));
        foreach (var path in pack.ModifiedAssets)
            File.Copy(Path.Join(tempPath, path), Path.Join(AppContext.BaseDirectory, path), true);

        Console.WriteLine("Завершение установки...");
        _config.Assets = _config.Assets.Where(a => !pack.DeletedAssets.Contains(a.FileName)).ToArray();
        AddAssetsToConfig(tempPath);
        Directory.Delete(tempPath, true);
        _config.InstalledReleaseId = _config.ReleaseId;
        await SaveConfig();

        Console.WriteLine("Обновление завершено!");
    }
}