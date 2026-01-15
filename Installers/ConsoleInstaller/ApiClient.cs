using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using ConsoleInstaller.Schemas;

namespace ConsoleInstaller;

public class ApiClient
{
    private readonly HttpClient _httpClient = new() { BaseAddress = new Uri(Config.ApiUrl.TrimEnd('\0')) };

    public async Task<ApplicationSchema> GetApplicationInfoAsync(string applicationId)
    {
        Console.WriteLine($"GET api/v1/apps/{applicationId}");
        var resp = await _httpClient.GetAsync($"GET api/v1/apps/{applicationId}");
        resp.EnsureSuccessStatusCode();
        var data = await resp.Content.ReadFromJsonAsync(ApiClientJsonSerializerContext.Default.ApplicationSchema);
        return data ?? throw new Exception("Invalid response");
    }

    public async Task<string> GetAssetsUrlAsync(string releaseId)
    {
        Console.WriteLine($"GET api/v1/releases/{releaseId}/assets/download");
        var resp = await _httpClient.GetAsync($"api/v1/releases/{releaseId}/assets/download");
        resp.EnsureSuccessStatusCode();
        var data = await resp.Content.ReadFromJsonAsync(ApiClientJsonSerializerContext.Default.UrlResponseSchema);
        return data?.Url ?? throw new Exception("Invalid response");
    }
}

[JsonSerializable(typeof(UrlResponseSchema))]
[JsonSerializable(typeof(ApplicationSchema))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
internal partial class ApiClientJsonSerializerContext : JsonSerializerContext
{
}