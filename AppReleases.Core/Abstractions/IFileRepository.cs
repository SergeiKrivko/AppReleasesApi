namespace AppReleases.Core.Abstractions;

public interface IFileRepository
{
    public Task<Stream> DownloadFileAsync(Guid fileId);
    public Task<Stream> DownloadFileAsync(Guid fileId, string extension);
    public Task DeleteFileAsync(Guid fileId);
    public Task DeleteFileAsync(Guid fileId, string extension);
    public Task<bool> FileExistsAsync(Guid fileId);
    public Task<bool> FileExistsAsync(Guid fileId, string extension);
    public Task UploadFileAsync(Guid fileId, Stream fileStream);
    public Task UploadFileAsync(Guid fileId, string extension, Stream fileStream);
    public Task<string> GetDownloadUrlAsync(Guid fileId, TimeSpan timeout);
    public Task<string> GetDownloadUrlAsync(Guid fileId, string extension, TimeSpan timeout);
}