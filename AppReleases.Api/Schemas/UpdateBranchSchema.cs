namespace AppReleases.Api.Schemas;

public class UpdateBranchSchema
{
    public TimeSpan? Duration { get; set; }
    public bool UseDefaultDuration { get; set; } = true;
}