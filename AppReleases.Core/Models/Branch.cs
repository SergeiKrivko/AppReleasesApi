using System.Text.Json.Serialization;
using AppReleases.Core.Json;

namespace AppReleases.Core.Models;

public class Branch
{
    public Guid Id { get; init; }
    public Guid ApplicationId { get; init; }
    public required string Name { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? DeletedAt { get; init; }

    [JsonConverter(typeof(TimeSpanConverter))]
    public TimeSpan? Duration { get; init; }

    public bool UseDefaultDuration { get; init; } = true;
}