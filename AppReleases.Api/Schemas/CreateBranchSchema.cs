namespace AppReleases.Api.Schemas;

public class CreateBranchSchema
{
    public required string Name { get; set; }
    public TimeSpan? Duration { get; set; }
    public bool UseDefaultDuration { get; set; } = true;
}