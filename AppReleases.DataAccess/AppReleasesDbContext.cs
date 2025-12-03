using AppReleases.DataAccess.Entities;
using AppReleases.Models;
using Microsoft.EntityFrameworkCore;

namespace AppReleases.DataAccess;

public class AppReleasesDbContext : DbContext
{
    public DbSet<TokenEntity> Tokens { get; init; }
    public DbSet<ApplicationEntity> Applications { get; init; }
    public DbSet<ReleaseEntity> Releases { get; init; }
    public DbSet<AssetEntity> Assets { get; init; }
    public DbSet<ReleaseAssetEntity> ReleaseAssets { get; init; }
    public DbSet<BranchEntity> Branches { get; init; }
    public DbSet<BundleEntity> Bundles { get; init; }
    public DbSet<InstallerBuilderUsageEntity> InstallerBuilderUsages { get; init; }
    public DbSet<BuiltInstallerEntity> BuiltInstallers { get; init; }

    public AppReleasesDbContext(DbContextOptions<AppReleasesDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppReleasesDbContext).Assembly);
    }
}