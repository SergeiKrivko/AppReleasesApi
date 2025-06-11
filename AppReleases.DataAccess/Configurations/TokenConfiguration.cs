using AppReleases.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppReleases.DataAccess.Configurations;

public class TokenConfiguration : IEntityTypeConfiguration<TokenEntity>
{
    public void Configure(EntityTypeBuilder<TokenEntity> builder)
    {
        builder.HasKey(x => x.TokenId);

        builder.Property(x => x.TokenId).IsRequired();
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.IssuedAt).IsRequired();
        builder.Property(x => x.ExpiresAt).IsRequired();
        builder.Property(x => x.RevokedAt);
        builder.Property(x => x.Mask).IsRequired();
    }
}