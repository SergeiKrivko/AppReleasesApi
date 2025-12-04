using System.IO.Compression;
using System.Security.Cryptography;
using AppReleases.Cli;
using AppReleases.Models;
using CommandLine;

var arguments = new Parser().ParseArguments<Arguments>(args).Value;

var client = new ApiClient(arguments.Url, arguments.Token);

var release = await client.CreateReleaseAsync(
    arguments.Application,
    arguments.Branch,
    arguments.Platform,
    Version.Parse(arguments.Version)
);

if (arguments.Directory is not null)
{
    var assets = Directory.EnumerateFiles(arguments.Directory, "*", SearchOption.AllDirectories)
        .Select(f => new AssetInfo
        {
            FileName = Path.GetRelativePath(arguments.Directory, f),
            FileHash = BitConverter.ToString(SHA256.HashData(File.ReadAllBytes(f))).Replace("-", ""),
        }).ToArray();

    Console.WriteLine("Uploading assets:");
    foreach (var asset in assets)
    {
        Console.WriteLine($"{asset.FileName} ----- {asset.FileHash}");
    }

    var difference = await client.GetReleaseDifferenceAsync(assets);

    using (var zipStream = new MemoryStream())
    {
        using (var zip = new ZipArchive(zipStream, ZipArchiveMode.Create))
        {
            foreach (var fileToUpload in difference.FilesToUpload)
            {
                await using var zipEntry = zip.CreateEntry(fileToUpload).Open();
                await using var stream = File.OpenRead(Path.Join(arguments.Directory, fileToUpload));
                await stream.CopyToAsync(zipEntry);
            }
        }

        await client.UploadReleaseAssets(release.Id, assets, new MemoryStream(zipStream.ToArray()));
    }
    Console.WriteLine();
}

foreach (var bundle in arguments.Bundles)
{
    Console.WriteLine($"Uploading bundle '{bundle}'...");
    await client.UploadReleaseBundle(release.Id, bundle);
}

Console.WriteLine("DONE");