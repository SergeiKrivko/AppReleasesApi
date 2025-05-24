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
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.ApplicationId).IsRequired();
        builder.Property(x => x.Version).IsRequired();
        builder.Property(x => x.Platform).IsRequired();
        builder.Property(x => x.ReleaseNotes);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.BranchId);
        builder.Property(x => x.IsObsolete).HasDefaultValue(false);
        builder.Property(x => x.IsPrerelease).HasDefaultValue(false);

        builder.HasOne(x => x.Application)
            .WithMany(x => x.Releases)
            .HasForeignKey(x => x.ApplicationId)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(x => x.Releases)
            .HasForeignKey(x => x.UserId)
            .IsRequired();
    }
}