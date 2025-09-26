namespace AppReleases.Models;

public class Metric
{
    public required Dictionary<string, string> Fields { get; set; }
    public string? Name { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Value { get; set; }
}