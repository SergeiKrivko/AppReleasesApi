using AppReleases.Core.Abstractions;
using AppReleases.Models;

namespace AppReleases.Application.Services;

public class BundleService(
    IBundleRepository bundleRepository,
    IFileRepository fileRepository) : IBundleService
{
    public Task<Bundle> GetBundleByIdAsync(Guid id)
    {
        return bundleRepository.GetBundleByIdAsync(id);
    }

    public Task<IEnumerable<Bundle>> GetAllBundlesAsync(Guid releaseId)
    {
        return bundleRepository.GetAllBundlesAsync(releaseId);
    }

    public Task<Bundle?> FindBundleAsync(Guid releaseId, string fileName)
    {
        return bundleRepository.FindBundleAsync(releaseId, fileName);
    }

    public async Task<Bundle> CreateBundleAsync(Guid releaseId, string fileName, Stream stream)
    {
        var bundle = new Bundle
        {
            BundleId = Guid.NewGuid(),
            ReleaseId = releaseId,
            FileId = Guid.NewGuid(),
            FileName = fileName,
            CreatedAt = DateTime.UtcNow,
        };
        await bundleRepository.CreateBundleAsync(bundle);
        await fileRepository.UploadFileAsync(FileRepositoryBucket.Bundles, bundle.FileId, fileName, stream);
        return bundle;
    }

    public async Task DeleteBundleAsync(Guid bundleId)
    {
        var bundle = await bundleRepository.GetBundleByIdAsync(bundleId);
        await fileRepository.DeleteFileAsync(FileRepositoryBucket.Bundles, bundle.FileId, bundle.FileName);
        await bundleRepository.DeleteBundleAsync(bundleId);
    }

    public async Task<string> GetDownloadUrlAsync(Guid bundleId)
    {
        var bundle = await bundleRepository.GetBundleByIdAsync(bundleId);
        return await fileRepository.GetDownloadUrlAsync(FileRepositoryBucket.Bundles, bundle.FileId,
            bundle.FileName, TimeSpan.FromHours(1));
    }
}