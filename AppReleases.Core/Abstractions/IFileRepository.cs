namespace AppReleases.Core.Abstractions;

public interface IFileRepository
{
    public Task<Stream> DownloadFileAsync(Guid fileId);
    public Task DeleteFileAsync(Guid fileId);
    public Task<bool> FileExistsAsync(Guid fileId);
    public Task UploadFileAsync(Guid fileId, Stream fileStream);
}