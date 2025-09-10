using AppReleases.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppReleases.DataAccess.Configurations;

public class InstallerConfiguration : IEntityTypeConfiguration<InstallerEntity>
{
    public void Configure(EntityTypeBuilder<InstallerEntity> builder)
    {
        builder.HasKey(x => x.InstallerId);

        builder.Property(x => x.InstallerId).IsRequired();
        builder.Property(x => x.ReleaseId).IsRequired();
        builder.Property(x => x.FileName).IsRequired();
        builder.Property(x => x.FileId).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.DeletedAt).HasDefaultValue(null);

        builder.HasOne(x => x.Release)
            .WithMany(x => x.Installers)
            .HasForeignKey(x => x.ReleaseId);
    }
}