using System.Net.Http.Json;
using AppReleases.Client.Schemas;

namespace AppReleases.Client;

internal class ApiClient(string apiUrl)
{
    private readonly HttpClient _httpClient = new() { BaseAddress = new Uri(apiUrl) };

    public async Task<ApplicationSchema> GetApplicationInfoAsync(Guid applicationId)
    {
        var resp = await _httpClient.GetAsync($"api/v1/apps/{applicationId}");
        resp.EnsureSuccessStatusCode();
        var data = await resp.Content.ReadFromJsonAsync(ReleasesJsonSerializerContext.Default.ApplicationSchema);
        return data ?? throw new Exception("Invalid response");
    }

    public async Task<ReleaseSchema> GetReleaseByIdAsync(Guid releaseId)
    {
        var resp = await _httpClient.GetAsync($"api/v1/releases/{releaseId}");
        resp.EnsureSuccessStatusCode();
        var data = await resp.Content.ReadFromJsonAsync(ReleasesJsonSerializerContext.Default.ReleaseSchema);
        return data ?? throw new Exception("Invalid response");
    }

    public async Task<ReleaseSchema> GetLatestReleaseAsync(Guid applicationId, Guid branchId, string? platform)
    {
        var resp = await _httpClient.GetAsync(
            $"api/v1/apps/{applicationId}/branches/{branchId}/releases/latest?platform={platform}");
        resp.EnsureSuccessStatusCode();
        var data = await resp.Content.ReadFromJsonAsync(ReleasesJsonSerializerContext.Default.ReleaseSchema);
        return data ?? throw new Exception("Invalid response");
    }
}