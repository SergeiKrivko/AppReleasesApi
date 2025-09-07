using System.Text.Json.Serialization;
using AppReleases.Models.Json;

namespace AppReleases.Models;

public class Application
{
    public required Guid Id { get; init; }
    public required string Key { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? DeletedAt { get; init; }
    public required string MainBranch { get; init; }
    [JsonConverter(typeof(TimeSpanConverter))]
    public TimeSpan? DefaultDuration { get; init; }
}