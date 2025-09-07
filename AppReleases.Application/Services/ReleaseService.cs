using System.IO.Compression;
using System.Text.Json;
using AppReleases.Core.Abstractions;
using AppReleases.Models;

namespace AppReleases.Application.Services;

public class ReleaseService(
    IReleaseRepository releaseRepository,
    IAssetRepository assetRepository,
    IFileRepository fileRepository) : IReleaseService
{
    public Task<Release> GetReleaseByIdAsync(Guid releaseId)
    {
        return releaseRepository.GetReleaseByIdAsync(releaseId);
    }

    public Task<Release?> GetLatestReleaseAsync(Guid branchId, string platform)
    {
        return releaseRepository.GetLatestReleaseAsync(branchId, platform);
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
        foreach (var asset in assets)
        {
            var existing = await assetRepository.FindAssetAsync(asset.FileName, asset.FileHash);
            if (existing == null)
            {
                var zipEntry = zip.GetEntry(asset.FileName);
                if (zipEntry == null)
                    throw new FileNotFoundException($"File '{asset.FileName}' not found");
                var memoryStream = new MemoryStream();
                await zipEntry.Open().CopyToAsync(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                existing = await UploadAssetAsync(asset, memoryStream);
            }

            await assetRepository.AddAssetToReleaseAsync(existing.Id, releaseId);
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
}