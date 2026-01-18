using System.Text.Json;
using AppReleases.Core.Abstractions;

namespace AppReleases.Installers.Shared;

public class InstallersHelper(InstallerBuilderContext context)
{
    private JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public string GenerateConfig()
    {
        return JsonSerializer.Serialize(new ConfigSchema
        {
            ApiUrl = context.ApiUrl ?? throw new Exception("ApiUrl is empty"),
            ApplicationId = context.Application.Id,
            InstalledReleaseId = context.Release.Id,
            Assets = context.Assets?.Select(a => new InstalledAssetSchema
            {
                FileName = a.FileName,
                FileHash = a.FileHash,
                InstalledFileName = a.FileName
            }).ToArray() ?? [],
        }, _jsonSerializerOptions);
    }

    public static async Task<byte[]> ReadStaticAsync(string name, CancellationToken token = default)
    {
        var filename = $"wwwroot/static/installers/{name}";
        return await File.ReadAllBytesAsync(Path.Join(AppContext.BaseDirectory, filename), token);
    }
}