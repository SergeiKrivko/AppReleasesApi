using AppReleases.Core.Enums;

namespace AppReleases.DataAccess.Entities;

public class TokenEntity
{
    public required Guid TokenId { get; init; }
    public required Guid UserId { get; init; }
    public required TokenType Type { get; init; }
    public required DateTime IssuedAt { get; init; }
    public required DateTime ExpiresAt { get; init; }
    public DateTime? RevokedAt { get; init; }
    public Guid? ApplicationId { get; init; }

    public virtual UserEntity User { get; init; }
    public virtual ApplicationEntity? Application { get; init; }
}