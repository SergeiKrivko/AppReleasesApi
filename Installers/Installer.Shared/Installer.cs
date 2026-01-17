using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Runtime.InteropServices;
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
        var tempPath = Path.Join(Path.GetTempPath(), Guid.NewGuid().ToString());
        ZipFile.ExtractToDirectory(stream, tempPath);
        return tempPath;
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
        if (_config is not null)
            _apiClient = new ApiClient(_config.ApiUrl);
    }

    private async Task DownloadUpdater(string sourceFileName, string destinationFileName)
    {
        ThrowIfNotInitialized();
        await using var stream = await _apiClient.DownloadStaticFileAsync(sourceFileName);
        await using var fileStream = new FileStream(destinationFileName, FileMode.Create);
        await stream.CopyToAsync(fileStream);
        if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
            File.SetUnixFileMode(destinationFileName,
                UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute | UnixFileMode.GroupRead |
                UnixFileMode.GroupExecute | UnixFileMode.OtherRead | UnixFileMode.OtherExecute);
    }

    private const string SystemRootDirectory = "__system_root__";

    private void InstallAssets(string assetsDirectory, string destinationDirectory)
    {
        ThrowIfNotInitialized();
        var assetsList = new List<InstalledAssetSchema>();

        Directory.CreateDirectory(destinationDirectory);
        var systemRoot = OperatingSystem.IsWindows()
            ? Environment.GetLogicalDrives().First()
            : "/";

        foreach (var file in Directory.EnumerateFiles(assetsDirectory, "*", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(assetsDirectory, file);
            var destinationPath = relativePath.StartsWith(SystemRootDirectory)
                ? systemRoot + relativePath.Substring(SystemRootDirectory.Length + 1)
                : Path.Join(destinationDirectory, relativePath);
            assetsList.Add(new InstalledAssetSchema
            {
                FileName = relativePath.StartsWith(SystemRootDirectory)
                    ? relativePath.Substring(SystemRootDirectory.Length)
                    : relativePath,
                InstalledFileName = destinationPath,
                FileHash = BitConverter.ToString(SHA256.HashData(File.ReadAllBytes(file))).Replace("-", "")
            });
            File.Move(file, destinationPath, true);
        }

        _config.Assets = _config.Assets.Concat(assetsList).ToArray();
    }

    public async Task InstallRelease()
    {
        ThrowIfNotInitialized();

        Console.WriteLine("Получение информации о приложении...");
        var application = await _apiClient.GetApplicationInfoAsync(_config.ApplicationId);
        var directory = PlatformsHelper.GetInstallPath(application);

        Console.WriteLine("Получение информации о релизе...");
        var release = await _apiClient.GetReleaseByIdAsync(_config.ReleaseId);

        Console.WriteLine("Подготовка к загрузке...");
        var url = await _apiClient.GetAssetsUrlAsync(_config.ReleaseId);

        Console.WriteLine("Загрузка и установка...");

        var tempDirectory = await DownloadAssets(url);
        InstallAssets(tempDirectory, directory);
        Directory.Delete(tempDirectory, true);

        Console.WriteLine("Завершение установки...");
        _config.InstalledReleaseId = _config.ReleaseId;
        _config.ReleaseId = Guid.Empty;
        await DownloadUpdater(
            $"Installer.Console.Updater_{RuntimeInformation.RuntimeIdentifier}.exe",
            Path.Join(directory, "Uninstall" + (OperatingSystem.IsWindows() ? ".exe" : "")));
        await SaveConfig(Path.Join(directory, ConfigFileName));

        Console.WriteLine("Установка завершена!");
    }

    public async Task UninstallRelease()
    {
        await ReadConfig();
        ThrowIfNotInitialized();
        foreach (var asset in _config.Assets)
        {
            File.Delete(asset.InstalledFileName);
        }

        File.Delete(ConfigPath);
    }

    public async Task UpdateRelease()
    {
        await ReadConfig();
        ThrowIfNotInitialized();

        Console.WriteLine("Получение информации об установленном релизе...");
        var installedRelease = await _apiClient.GetReleaseByIdAsync(_config.InstalledReleaseId);

        Console.WriteLine("Получение информации о последнем релизе...");
        var latestRelease = await _apiClient.GetLatestReleaseAsync(_config.ApplicationId, installedRelease.BranchId,
            installedRelease.Platform ?? throw new Exception("Platform is null"));
        if (latestRelease.Version <= installedRelease.Version)
        {
            Console.WriteLine("Уже установлена последняя версия приложения. Обновление не требуется.");
            return;
        }

        Console.WriteLine("Подготовка к загрузке...");
        var pack = await _apiClient.GetAssetsPackAsync(latestRelease.Id, _config.Assets.Select(asset => new AssetSchema
        {
            FileName = asset.FileName,
            FileHash = BitConverter.ToString(SHA256.HashData(File.ReadAllBytes(asset.InstalledFileName)))
                .Replace("-", "")
        }));

        Console.WriteLine("Загрузка обновления...");
        var tempPath = await DownloadAssets(pack.Url);

        Console.WriteLine("Установка обновления...");
        foreach (var path in pack.DeletedAssets)
            File.Delete(Path.IsPathRooted(path) ? path : Path.Join(AppContext.BaseDirectory, path));
        InstallAssets(tempPath, AppContext.BaseDirectory);

        Console.WriteLine("Завершение установки...");
        _config.Assets = _config.Assets.Where(a => !pack.DeletedAssets.Contains(a.FileName)).ToArray();
        Directory.Delete(tempPath, true);
        _config.InstalledReleaseId = latestRelease.Id;
        await SaveConfig();

        Console.WriteLine("Обновление завершено!");
    }
}