namespace AppReleases.Api.Schemas;

public class CreateReleaseSchema
{
    public required string ApplicationKey { get; init; }
    public string? Branch { get; init; }
    public required string Platform { get; init; }
    public required Version Version { get; init; }
}