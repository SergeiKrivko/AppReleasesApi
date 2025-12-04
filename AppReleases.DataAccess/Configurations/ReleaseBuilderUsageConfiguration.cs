using AppReleases.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppReleases.DataAccess.Configurations;

public class ReleaseBuilderUsageConfiguration : IEntityTypeConfiguration<InstallerBuilderUsageEntity>
{
    public void Configure(EntityTypeBuilder<InstallerBuilderUsageEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder.Property(x => x.ApplicationId).IsRequired();
        builder.Property(x => x.BuilderKey).IsRequired();
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.CreatedAt);
        builder.Property(x => x.DeletedAt);
        builder.Property(x => x.InstallerLifetime);
        builder.Property(x => x.Platforms);
        builder.Property(x => x.Settings)
            .HasColumnType("jsonb");

        builder.HasOne(x => x.Application)
            .WithMany(x => x.InstallerBuilders)
            .HasForeignKey(x => x.ApplicationId);
    }
}