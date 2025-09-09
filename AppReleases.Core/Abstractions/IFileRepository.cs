namespace AppReleases.Core.Abstractions;

public interface IFileRepository
{
    public Task<Stream> DownloadFileAsync(FileRepositoryBucket bucket, Guid fileId);
    public Task<Stream> DownloadFileAsync(FileRepositoryBucket bucket, Guid fileId, string extension);
    public Task DeleteFileAsync(FileRepositoryBucket bucket, Guid fileId);
    public Task DeleteFileAsync(FileRepositoryBucket bucket, Guid fileId, string extension);
    public Task<bool> FileExistsAsync(FileRepositoryBucket bucket, Guid fileId);
    public Task<bool> FileExistsAsync(FileRepositoryBucket bucket, Guid fileId, string extension);
    public Task UploadFileAsync(FileRepositoryBucket bucket, Guid fileId, Stream fileStream);
    public Task UploadFileAsync(FileRepositoryBucket bucket, Guid fileId, string extension, Stream fileStream);
    public Task<string> GetDownloadUrlAsync(FileRepositoryBucket bucket, Guid fileId, TimeSpan timeout);
    public Task<string> GetDownloadUrlAsync(FileRepositoryBucket bucket, Guid fileId, string extension, TimeSpan timeout);
    public Task<int> ClearFilesCreatedBefore(FileRepositoryBucket bucket, DateTime beforeDate, CancellationToken cancellationToken);
}

public enum FileRepositoryBucket
{
    Assets,
    Temp
}