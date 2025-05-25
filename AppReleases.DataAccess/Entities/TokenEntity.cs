using System.ComponentModel.DataAnnotations;

namespace AppReleases.DataAccess.Entities;

public class TokenEntity
{
    public required Guid TokenId { get; init; }
    [MaxLength(256)] public required string Name { get; init; }
    public required DateTime IssuedAt { get; init; }
    public required DateTime ExpiresAt { get; init; }
    public DateTime? RevokedAt { get; init; }
    public Guid? ApplicationId { get; init; }

    public virtual ApplicationEntity? Application { get; init; }
}