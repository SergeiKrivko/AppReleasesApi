using AppReleases.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppReleases.DataAccess.Configurations;

public class ApplicationConfiguration : IEntityTypeConfiguration<ApplicationEntity>
{
    public void Configure(EntityTypeBuilder<ApplicationEntity> builder)
    {
        builder.HasKey(x => x.ApplicationId);

        builder.Property(x => x.ApplicationId).IsRequired();
        builder.Property(x => x.Key).IsRequired();
        builder.Property(x => x.Name);
        builder.Property(x => x.Description);
        builder.Property(x => x.MainBranch).IsRequired();
        builder.Property(x => x.DefaultDuration).HasDefaultValue(null);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.DeletedAt);

        builder.HasMany(x => x.Branches)
            .WithOne(x => x.Application)
            .HasForeignKey(x => x.ApplicationId)
            .IsRequired();
    }
}