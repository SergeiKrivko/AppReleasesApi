namespace Installer.Shared.Schemas;

public class ApplicationSchema
{
    public required Guid Id { get; init; }
    public required string Key { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? DeletedAt { get; init; }
    public required string MainBranch { get; init; }
}