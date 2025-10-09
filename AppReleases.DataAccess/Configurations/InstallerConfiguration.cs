using AppReleases.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppReleases.DataAccess.Configurations;

public class BundleConfiguration : IEntityTypeConfiguration<BundleEntity>
{
    public void Configure(EntityTypeBuilder<BundleEntity> builder)
    {
        builder.HasKey(x => x.BundleId);

        builder.Property(x => x.BundleId).IsRequired();
        builder.Property(x => x.ReleaseId).IsRequired();
        builder.Property(x => x.FileName).IsRequired();
        builder.Property(x => x.FileId).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.DeletedAt).HasDefaultValue(null);

        builder.HasOne(x => x.Release)
            .WithMany(x => x.Bundles)
            .HasForeignKey(x => x.ReleaseId);
    }
}