using System.Diagnostics;
using System.Text.Json;
using AppReleases.Client.Schemas;

namespace AppReleases.Client;

public class AvaluxReleasesClient
{
    private readonly ConfigSchema _config;
    private readonly ApiClient _apiClient;

    public AvaluxReleasesClient()
    {
        _config = JsonSerializer.Deserialize(
            File.ReadAllText(Path.Join(AppContext.BaseDirectory, "InstallerConfig.json")),
            ReleasesJsonSerializerContext.Default.ConfigSchema) ?? throw new Exception("Unable to read config");
        _apiClient = new ApiClient(_config.ApiUrl);
    }

    /// <summary>
    /// Получение информации о текущем приложении.
    /// </summary>
    /// <returns>ApplicationSchema</returns>
    public async Task<ApplicationSchema> GetApplicationInfoAsync()
    {
        return await _apiClient.GetApplicationInfoAsync(_config.ApplicationId);
    }

    /// <summary>
    /// Получение информации об установленном в данный момент релизе.
    /// </summary>
    /// <returns>ReleaseSchema</returns>
    public async Task<ReleaseSchema> GetInstalledReleaseAsync()
    {
        return await _apiClient.GetReleaseByIdAsync(_config.InstalledReleaseId);
    }

    /// <summary>
    /// Получение информации о последнем (самом новом) релизе.
    /// Если уже установлен последний релиз, вернет null.
    /// </summary>
    /// <returns>ReleaseSchema</returns>
    public async Task<ReleaseSchema?> GetLatestReleaseAsync()
    {
        var installedRelease = await GetInstalledReleaseAsync();
        var latestRelease = await _apiClient.GetLatestReleaseAsync(_config.ApplicationId, installedRelease.BranchId,
            installedRelease.Platform);
        if (latestRelease.Version <= installedRelease.Version)
            return null;
        return latestRelease;
    }

    /// <summary>
    /// Запуск обновления приложения. После вызова этого метода приложение должно завершить работу как можно быстрее.
    /// </summary>
    public void StartUpdateApplication()
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = Path.Join(AppContext.BaseDirectory, OperatingSystem.IsWindows() ? "Uninstall.exe" : "Uninstall"),
            Arguments = "--update",
            UseShellExecute = true,
        });
    }

    /// <summary>
    /// Запуск удаления приложения. После вызова этого метода приложение должно завершить работу как можно быстрее.
    /// </summary>
    public void StartUninstallApplication()
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = Path.Join(AppContext.BaseDirectory, OperatingSystem.IsWindows() ? "Uninstall.exe" : "Uninstall"),
            Arguments = "",
            UseShellExecute = true,
        });
    }
}