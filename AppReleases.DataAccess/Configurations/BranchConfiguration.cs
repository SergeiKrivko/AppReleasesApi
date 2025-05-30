﻿using AppReleases.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppReleases.DataAccess.Configurations;

public class BranchConfiguration : IEntityTypeConfiguration<BranchEntity>
{
    public void Configure(EntityTypeBuilder<BranchEntity> builder)
    {
        builder.HasKey(x => x.BranchId);

        builder.Property(x => x.BranchId).IsRequired();
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.ApplicationId).IsRequired();
        builder.Property(x => x.IsMerged).HasDefaultValue(false);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.DeletedAt);

        builder.HasMany(x => x.Releases)
            .WithOne(x => x.Branch)
            .HasForeignKey(x => x.BranchId);

        builder.HasOne(x => x.Application)
            .WithMany(x => x.Branches)
            .HasForeignKey(x => x.ApplicationId)
            .IsRequired();
    }
}