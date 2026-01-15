using System.Net.Http.Json;
using Installer.Shared.Schemas;

namespace Installer.Shared;

public class ApiClient(string apiUrl)
{
    private readonly HttpClient _httpClient = new() { BaseAddress = new Uri(apiUrl) };

    public async Task<ApplicationSchema> GetApplicationInfoAsync(Guid applicationId)
    {
        Console.WriteLine($"GET api/v1/apps/{applicationId}");
        var resp = await _httpClient.GetAsync($"api/v1/apps/{applicationId}");
        resp.EnsureSuccessStatusCode();
        var data = await resp.Content.ReadFromJsonAsync(InstallerJsonSerializerContext.Default.ApplicationSchema);
        return data ?? throw new Exception("Invalid response");
    }

    public async Task<string> GetAssetsUrlAsync(Guid releaseId)
    {
        Console.WriteLine($"GET api/v1/releases/{releaseId}/assets/download");
        var resp = await _httpClient.GetAsync($"api/v1/releases/{releaseId}/assets/download");
        resp.EnsureSuccessStatusCode();
        var data = await resp.Content.ReadFromJsonAsync(InstallerJsonSerializerContext.Default.UrlResponseSchema);
        return data?.Url ?? throw new Exception("Invalid response");
    }

    public async Task<ReleaseSchema> GetLatestReleaseAsync(Guid applicationId)
    {
        Console.WriteLine($"GET api/v1/apps/{applicationId}/releases/latest");
        var resp = await _httpClient.GetAsync($"api/v1/apps/{applicationId}/releases/latest");
        resp.EnsureSuccessStatusCode();
        var data = await resp.Content.ReadFromJsonAsync(InstallerJsonSerializerContext.Default.ReleaseSchema);
        return data ?? throw new Exception("Invalid response");
    }

    public async Task<AssetsPackSchema> GetAssetsPackAsync(Guid releaseId, IEnumerable<AssetSchema> assets)
    {
        Console.WriteLine($"POST api/v1/releases/{releaseId}/assets/download");
        var resp = await _httpClient.PostAsync($"api/v1/releases/{releaseId}/assets/download",
            JsonContent.Create(assets));
        resp.EnsureSuccessStatusCode();
        var data = await resp.Content.ReadFromJsonAsync(InstallerJsonSerializerContext.Default.AssetsPackSchema);
        return data ?? throw new Exception("Invalid response");
    }
}