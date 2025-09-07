using AppReleases.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppReleases.DataAccess.Configurations;

public class ReleaseConfiguration : IEntityTypeConfiguration<ReleaseEntity>
{
    public void Configure(EntityTypeBuilder<ReleaseEntity> builder)
    {
        builder.HasKey(x => x.ReleaseId);

        builder.Property(x => x.ReleaseId).IsRequired();
        builder.Property(x => x.BranchId).IsRequired();
        builder.Property(x => x.Version).IsRequired();
        builder.Property(x => x.Platform).IsRequired();
        builder.Property(x => x.ReleaseNotes);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.DeletedAt).HasDefaultValue(null);

        builder.HasMany(x => x.Assets)
            .WithOne(x => x.Release)
            .HasForeignKey(x => x.ReleaseId)
            .IsRequired();
    }
}