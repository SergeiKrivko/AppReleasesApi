using AppReleases.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppReleases.DataAccess.Configurations;

public class AssetConfiguration : IEntityTypeConfiguration<AssetEntity>
{
    public void Configure(EntityTypeBuilder<AssetEntity> builder)
    {
        builder.HasKey(x => x.AssetId);

        builder.Property(x => x.AssetId).IsRequired();
        builder.Property(x => x.ReleaseId).IsRequired();
        builder.Property(x => x.FileName).IsRequired();
        builder.Property(x => x.FileHash).IsRequired();
        builder.Property(x => x.FileId).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
    }
}