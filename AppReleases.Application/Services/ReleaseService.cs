using System.IO.Compression;
using System.Text.Json;
using AppReleases.Core.Abstractions;
using AppReleases.Models;
using Microsoft.Extensions.Logging;

namespace AppReleases.Application.Services;

public class ReleaseService(
    IReleaseRepository releaseRepository,
    IAssetRepository assetRepository,
    IFileRepository fileRepository,
    IBundleService bundleService,
    ILogger<ReleaseService> logger) : IReleaseService
{
    public Task<Release> GetReleaseByIdAsync(Guid releaseId)
    {
        return releaseRepository.GetReleaseByIdAsync(releaseId);
    }

    public Task<Release?> GetLatestReleaseAsync(Guid branchId, string platform)
    {
        return releaseRepository.GetLatestReleaseAsync(branchId, platform);
    }

    public Task<IEnumerable<Release>> GetAllReleasesOfApplicationAsync(Guid applicationId)
    {
        return releaseRepository.GetAllReleasesOfApplicationAsync(applicationId);
    }

    public Task<IEnumerable<Release>> GetAllReleasesOfBranchAsync(Guid branchId)
    {
        return releaseRepository.GetAllReleasesOfBranchAsync(branchId);
    }

    public Task<Release?> FindReleaseAsync(Guid branchId, string? platform, Version version)
    {
        return releaseRepository.FindReleaseAsync(branchId, platform, version);
    }

    public async Task<ReleaseDifference> GetReleaseDifferenceAsync(AssetInfo[] assets)
    {
        var toUpload = new List<string>();
        foreach (var asset in assets)
        {
            var existing = await assetRepository.FindAssetAsync(asset.FileName, asset.FileHash);
            if (existing == null)
                toUpload.Add(asset.FileName);
        }

        return new ReleaseDifference
        {
            FilesToUpload = toUpload.ToArray(),
        };
    }

    public async Task UploadAssetsAsync(Guid releaseId, AssetInfo[] assets, Stream zipStream)
    {
        using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);
        int count = 0;
        foreach (var asset in assets)
        {
            logger.LogInformation("{done} / {all} --- Processing asset {asset}", count, assets.Length, asset.FileName);
            var existing = await assetRepository.FindAssetAsync(asset.FileName, asset.FileHash);
            if (existing == null)
            {
                var zipEntry = zip.GetEntry(asset.FileName);
                if (zipEntry == null)
                    throw new FileNotFoundException($"File '{asset.FileName}' not found");
                using var memoryStream = new MemoryStream();
                logger.LogInformation("{done} / {all} --- Extracting asset {asset} from zip", count, assets.Length, asset.FileName);
                await using (var entryStream = zipEntry.Open())
                    await entryStream.CopyToAsync(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                logger.LogInformation("{done} / {all} --- Uploading asset {asset} to S3", count, assets.Length, asset.FileName);
                existing = await UploadAssetAsync(asset, memoryStream);
            }

            await assetRepository.AddAssetToReleaseAsync(existing.Id, releaseId);
            count++;
            logger.LogInformation("{done} / {all} --- Asset {asset} uploaded", count, assets.Length, asset.FileName);
        }
    }

    private async Task<Asset> UploadAssetAsync(AssetInfo assetInfo, Stream stream)
    {
        var asset = new Asset
        {
            Id = Guid.NewGuid(),
            FileName = assetInfo.FileName,
            FileHash = assetInfo.FileHash,
            CreatedAt = DateTime.UtcNow,
            FileId = Guid.NewGuid(),
        };
        await assetRepository.CreateAssetAsync(asset);
        await fileRepository.UploadFileAsync(FileRepositoryBucket.Assets, asset.FileId, stream);
        return asset;
    }

    public async Task<Release> CreateReleaseAsync(Guid branchId, string? platform, Version version)
    {
        var release = new Release
        {
            Id = Guid.NewGuid(),
            BranchId = branchId,
            Platform = platform,
            Version = version,
            CreatedAt = DateTime.UtcNow
        };
        await releaseRepository.CreateReleaseAsync(release);
        return release;
    }

    public Task UpdateReleaseAsync(Guid releaseId, string? description)
    {
        return releaseRepository.UpdateReleaseAsync(releaseId, description);
    }

    public async Task DeleteReleaseAsync(Guid releaseId)
    {
        foreach (var bundle in await bundleService.GetAllBundlesAsync(releaseId))
        {
            await bundleService.DeleteBundleAsync(bundle.BundleId);
        }
        await releaseRepository.DeleteReleaseAsync(releaseId);
    }

    private TimeSpan AssetsZipLifetime { get; } = TimeSpan.FromHours(1);

    public async Task<string> PackAssetsAsync(Guid releaseId)
    {
        var assets = await assetRepository.GetAllAssetsAsync(releaseId);
        return await PackAssetsZipAsync(assets);
    }

    private async Task<string> PackAssetsZipAsync(IEnumerable<Asset> assets)
    {
        var tempFileId = Guid.NewGuid();

        using (var zipStream = new MemoryStream())
        {
            using (var zip = new ZipArchive(zipStream, ZipArchiveMode.Create))
            {
                foreach (var asset in assets)
                {
                    await using var zipEntry = zip.CreateEntry(asset.FileName).Open();
                    await using var stream =
                        await fileRepository.DownloadFileAsync(FileRepositoryBucket.Assets, asset.FileId);
                    await stream.CopyToAsync(zipEntry);
                }
            }

            await fileRepository.UploadFileAsync(FileRepositoryBucket.Temp, tempFileId, $"{tempFileId}.zip",
                new MemoryStream(zipStream.ToArray()));
        }

        return await fileRepository.GetDownloadUrlAsync(FileRepositoryBucket.Temp, tempFileId, $"{tempFileId}.zip",
            AssetsZipLifetime);
    }

    public async Task<IEnumerable<AssetInfo>> ListAssetsAsync(Guid releaseId)
    {
        var assets = await assetRepository.GetAllAssetsAsync(releaseId);
        return assets.Select(a => new AssetInfo { FileName = a.FileName, FileHash = a.FileHash });
    }

    public async Task<AssetsPack> PackAssetsAsync(Guid releaseId, AssetInfo[] existingAssets)
    {
        var assets = await assetRepository.GetAllAssetsAsync(releaseId);
        var deletedAssets = existingAssets
            .Where(a => assets.FirstOrDefault(e => e.FileName == a.FileName && e.FileHash == a.FileHash) == null);
        var modifiedAssets = assets
            .Where(a => existingAssets.FirstOrDefault(e => e.FileName == a.FileName && e.FileHash == a.FileHash) ==
                        null)
            .ToList();
        var zipUrl = await PackAssetsZipAsync(modifiedAssets);
        return new AssetsPack
        {
            Url = zipUrl,
            DeletedAssets = deletedAssets.Select(a => a.FileName).ToArray(),
            ModifiedAssets = modifiedAssets.Select(a => a.FileName).ToArray(),
        };
    }
}