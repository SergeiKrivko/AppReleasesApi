namespace AppReleases.Api.Schemas;

public class CreateTokenSchema
{
    public required string Name { get; init; }
    public required string Mask { get; init; }
    public DateTime? ExpiresAt { get; init; }
}