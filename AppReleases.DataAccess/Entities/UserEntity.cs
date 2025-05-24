using System.ComponentModel.DataAnnotations;

namespace AppReleases.DataAccess.Entities;

public class UserEntity
{
    public Guid UserId { get; init; }
    [MaxLength(32)] public required string Login { get; init; }
    [MaxLength(100)] public required string PasswordHash { get; init; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? DeletedAt { get; init; }

    public virtual ICollection<TokenEntity> Tokens { get; init; } = [];
    public virtual ICollection<ApplicationEntity> Applications { get; init; } = [];
    public virtual ICollection<ReleaseEntity> Releases { get; init; } = [];
}