using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using ConsoleInstaller.Schemas;

namespace ConsoleInstaller;

public class ApiClient
{
    private readonly HttpClient _httpClient = new() { BaseAddress = new Uri(Config.ApiUrl.TrimEnd('\0')) };

    public async Task<string> GetAssetsUrlAsync(string releaseId)
    {
        Console.WriteLine($"GET api/v1/releases/{releaseId}/assets/download");
        var resp = await _httpClient.GetAsync($"api/v1/releases/{releaseId}/assets/download");
        Console.WriteLine(resp.StatusCode);
        var data = await resp.Content.ReadFromJsonAsync(ApiClientJsonSerializerContext.Default.UrlResponseSchema);
        return data?.Url ?? throw new Exception("Invalid response");
    }
}

[JsonSerializable(typeof(UrlResponseSchema))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
internal partial class ApiClientJsonSerializerContext : JsonSerializerContext
{
}