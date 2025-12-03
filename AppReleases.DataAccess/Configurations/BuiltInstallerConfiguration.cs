using AppReleases.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppReleases.DataAccess.Configurations;

public class BuiltInstallerConfiguration : IEntityTypeConfiguration<BuiltInstallerEntity>
{
    public void Configure(EntityTypeBuilder<BuiltInstallerEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder.Property(x => x.ReleaseId).IsRequired();
        builder.Property(x => x.BuilderId).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.DownloadedAt);
        builder.Property(x => x.DeletedAt);
        builder.Property(x => x.FileId);
        builder.Property(x => x.FileName);

        builder.HasOne(x => x.Release)
            .WithMany(x => x.BuiltInstallers)
            .HasForeignKey(x => x.ReleaseId);
    }
}