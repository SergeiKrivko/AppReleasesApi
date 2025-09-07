namespace AppReleases.Cli.Schemas;

public class CreateReleaseSchema
{
    public required string ApplicationKey { get; init; }
    public string? Branch { get; init; }
    public string? Platform { get; init; }
    public required Version Version { get; init; }
}