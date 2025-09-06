namespace AppReleases.Api.Schemas;

public class CreateApplicationSchema
{
    public required string Key { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public string? MainBranch { get; set; }
}