using System.Net.Http.Headers;
using System.Net.Http.Json;
using AppReleases.Cli.Schemas;
using AppReleases.Models;

namespace AppReleases.Cli;

public class ApiClient(string baseUrl, string apiKey)
{
    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri(baseUrl),
        DefaultRequestHeaders = { Authorization = new AuthenticationHeaderValue("Bearer", apiKey) }
    };

    public async Task<Release> CreateReleaseAsync(string key, string? branch, string? platform, Version version)
    {
        var resp = await _httpClient.PostAsJsonAsync("api/v1/releases", new CreateReleaseSchema
        {
            ApplicationKey = key,
            Platform = platform,
            Version = version,
            Branch = branch
        });
        resp.EnsureSuccessStatusCode();

        var release = await resp.Content.ReadFromJsonAsync<Release>();
        return release ?? throw new NullReferenceException("API return unprocessable response");
    }

    public async Task<ReleaseDifference> GetReleaseDifferenceAsync(AssetInfo[] assets)
    {
        var resp = await _httpClient.PostAsJsonAsync("api/v1/releases/diff", assets);
        resp.EnsureSuccessStatusCode();
        var release = await resp.Content.ReadFromJsonAsync<ReleaseDifference>();
        return release ?? throw new NullReferenceException("API return unprocessable response");
    }

    public async Task UploadReleaseAssets(Guid releaseId, AssetInfo[] assets, Stream assetsZipStream)
    {
        var content = new MultipartFormDataContent();
        content.Add(new StreamContent(assetsZipStream), "zip", "assets.zip");
        content.Add(JsonContent.Create(assets), "assets");
        var resp = await _httpClient.PutAsync($"api/v1/releases/{releaseId}/assets", content);
        resp.EnsureSuccessStatusCode();
    }
}