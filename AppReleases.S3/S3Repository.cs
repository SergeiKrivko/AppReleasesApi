using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using AppReleases.Core.Abstractions;

namespace AppReleases.S3;

public class S3Repository : IFileRepository
{
    private readonly AmazonS3Client _s3Client;

    private static string MainBucket { get; } = Environment.GetEnvironmentVariable("S3_ASSETS_BUCKET") ?? "assets";
    private static string ZipBucket { get; } = Environment.GetEnvironmentVariable("S3_TEMP_BUCKET") ?? "temp";

    public S3Repository()
    {
        _s3Client = new AmazonS3Client(
            new BasicAWSCredentials(Environment.GetEnvironmentVariable("S3_ACCESS_KEY"),
                Environment.GetEnvironmentVariable("S3_SECRET_KEY")),
            new AmazonS3Config
            {
                ServiceURL = Environment.GetEnvironmentVariable("S3_SERVICE_URL"),
                AuthenticationRegion = Environment.GetEnvironmentVariable("S3_AUTHORIZATION_REGION"),
                ForcePathStyle = true
            });
    }

    public Task<Stream> DownloadFileAsync(Guid fileId)
    {
        return DownloadFileAsync(fileId.ToString());
    }

    public Task<Stream> DownloadFileAsync(Guid fileId, string extension)
    {
        return DownloadFileAsync($"{fileId}.{extension}");
    }

    public Task DeleteFileAsync(Guid fileId)
    {
        return DeleteFileAsync(fileId.ToString());
    }

    public Task DeleteFileAsync(Guid fileId, string extension)
    {
        return DeleteFileAsync($"{fileId}.{extension}");
    }

    public Task<bool> FileExistsAsync(Guid fileId)
    {
        return FileExistsAsync(fileId.ToString());
    }

    public Task<bool> FileExistsAsync(Guid fileId, string extension)
    {
        return FileExistsAsync($"{fileId}.{extension}");
    }

    public Task UploadFileAsync(Guid fileId, Stream fileStream)
    {
        return UploadFileAsync(fileId.ToString(), fileStream);
    }

    public Task UploadFileAsync(Guid fileId, string extension, Stream fileStream)
    {
        return UploadFileAsync($"{fileId}.{extension}", fileStream);
    }

    public Task<string> GetDownloadUrlAsync(Guid fileId, TimeSpan timeout)
    {
        return GetDownloadUrlAsync(fileId.ToString(), timeout);
    }

    public Task<string> GetDownloadUrlAsync(Guid fileId, string extension, TimeSpan timeout)
    {
        return GetDownloadUrlAsync($"{fileId}.{extension}", timeout);
    }

    private async Task<Stream> DownloadFileAsync(string fileName)
    {
        return (await _s3Client.GetObjectAsync(MainBucket, fileName))
            .ResponseStream;
    }

    private Task DeleteFileAsync(string fileName)
    {
        throw new NotImplementedException();
    }

    private Task<bool> FileExistsAsync(string fileName)
    {
        throw new NotImplementedException();
    }

    private Task UploadFileAsync(string fileName, Stream fileStream)
    {
        var putRequest = new PutObjectRequest
        {
            BucketName = MainBucket,
            Key = fileName,
            InputStream = fileStream,
            ContentType = "application/octet-stream"
        };

        return _s3Client.PutObjectAsync(putRequest);
    }

    private async Task<string> GetDownloadUrlAsync(string fileName, TimeSpan timeout)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = ZipBucket,
            Key = fileName,
            Expires = DateTime.UtcNow.Add(timeout)
        };
        return await _s3Client.GetPreSignedURLAsync(request);
    }
}