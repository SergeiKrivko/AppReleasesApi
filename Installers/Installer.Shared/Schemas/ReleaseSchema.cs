namespace Installer.Shared.Schemas;

public class ReleaseSchema
{
    public required Guid Id { get; init; }
    public required Guid BranchId { get; init; }
    public required Version Version { get; init; }
    public string? Platform { get; init; }
    public string? ReleaseNotes { get; set; }
}