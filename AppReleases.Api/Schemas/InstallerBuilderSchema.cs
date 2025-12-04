namespace AppReleases.Api.Schemas;

public class InstallerBuilderSchema
{
    public required string Key { get; init; }
    public required string DisplayName { get; init; }
    public string? Description { get; init; }
}