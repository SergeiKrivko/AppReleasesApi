using System.Text.Json;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using AppReleases.Core.Abstractions;

namespace AppReleases.S3;

public class S3Repository : IFileRepository
{
    private readonly AmazonS3Client _s3Client;

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

    public Task<Stream> DownloadFileAsync(FileRepositoryBucket bucket, Guid fileId)
    {
        return DownloadFileAsync(GetBucket(bucket), fileId.ToString());
    }

    public Task<Stream> DownloadFileAsync(FileRepositoryBucket bucket, Guid fileId, string extension)
    {
        return DownloadFileAsync(GetBucket(bucket), $"{fileId}.{extension}");
    }

    public Task DeleteFileAsync(FileRepositoryBucket bucket, Guid fileId)
    {
        return DeleteFileAsync(GetBucket(bucket), fileId.ToString());
    }

    public Task DeleteFileAsync(FileRepositoryBucket bucket, Guid fileId, string extension)
    {
        return DeleteFileAsync(GetBucket(bucket), $"{fileId}.{extension}");
    }

    public Task<bool> FileExistsAsync(FileRepositoryBucket bucket, Guid fileId)
    {
        return FileExistsAsync(GetBucket(bucket), fileId.ToString());
    }

    public Task<bool> FileExistsAsync(FileRepositoryBucket bucket, Guid fileId, string extension)
    {
        return FileExistsAsync(GetBucket(bucket), $"{fileId}.{extension}");
    }

    public Task UploadFileAsync(FileRepositoryBucket bucket, Guid fileId, Stream fileStream)
    {
        return UploadFileAsync(GetBucket(bucket), fileId.ToString(), fileStream);
    }

    public Task UploadFileAsync(FileRepositoryBucket bucket, Guid fileId, string extension, Stream fileStream)
    {
        return UploadFileAsync(GetBucket(bucket), $"{fileId}.{extension}", fileStream);
    }

    public Task<string> GetDownloadUrlAsync(FileRepositoryBucket bucket, Guid fileId, TimeSpan timeout)
    {
        return GetDownloadUrlAsync(GetBucket(bucket), fileId.ToString(), timeout);
    }

    public Task<string> GetDownloadUrlAsync(FileRepositoryBucket bucket, Guid fileId, string extension,
        TimeSpan timeout)
    {
        return GetDownloadUrlAsync(GetBucket(bucket), $"{fileId}.{extension}", timeout);
    }

    private async Task<Stream> DownloadFileAsync(string bucket, string fileName)
    {
        return (await _s3Client.GetObjectAsync(bucket, fileName))
            .ResponseStream;
    }

    private async Task DeleteFileAsync(string bucket, string fileName)
    {
        var deleteRequest = new DeleteObjectRequest
        {
            BucketName = bucket,
            Key = fileName,
        };
        await _s3Client.DeleteObjectAsync(deleteRequest);
    }

    private async Task<bool> FileExistsAsync(string bucket, string fileName)
    {
        try
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = bucket,
                Key = fileName
            };

            await _s3Client.GetObjectMetadataAsync(request);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    private async Task UploadFileAsync(string bucket, string fileName, Stream fileStream)
    {
        var putRequest = new PutObjectRequest
        {
            BucketName = bucket,
            Key = fileName,
            InputStream = fileStream,
            ContentType = "application/octet-stream"
        };

        await _s3Client.PutObjectAsync(putRequest);
    }

    private async Task<string> GetDownloadUrlAsync(string bucket, string fileName, TimeSpan timeout)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucket,
            Key = fileName,
            Expires = DateTime.UtcNow.Add(timeout)
        };
        return await _s3Client.GetPreSignedURLAsync(request);
    }

    public async Task<int> ClearFilesCreatedBefore(FileRepositoryBucket bucket, DateTime beforeDate,
        CancellationToken cancellationToken)
    {
        var count = 0;
        var bucketName = GetBucket(bucket);
        var files = await _s3Client.ListObjectsAsync(bucketName, cancellationToken);
        foreach (var file in files?.S3Objects ?? [])
        {
            if (file != null && file.LastModified < beforeDate)
            {
                await DeleteFileAsync(bucketName, file.Key);
                count++;
            }
        }
        return count;
    }

    private static string AssetsBucket { get; } = Environment.GetEnvironmentVariable("S3_ASSETS_BUCKET") ?? "assets";
    private static string TempBucket { get; } = Environment.GetEnvironmentVariable("S3_TEMP_BUCKET") ?? "temp";

    private static string GetBucket(FileRepositoryBucket bucket)
    {
        return bucket switch
        {
            FileRepositoryBucket.Assets => AssetsBucket,
            FileRepositoryBucket.Temp => TempBucket,
            _ => throw new ArgumentOutOfRangeException(nameof(bucket), bucket, null)
        };
    }
}