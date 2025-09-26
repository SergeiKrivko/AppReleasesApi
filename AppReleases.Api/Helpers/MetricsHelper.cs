using System.Diagnostics;
using AppReleases.Core.Abstractions;
using Prometheus;

namespace AppReleases.Api.Helpers;

public class MetricsHelper : IMetricsHelper
{
    private readonly Histogram _downloadAssetsHistogram = Metrics.CreateHistogram(
        "download_assets_duration_seconds",
        "Histogram of download assets duration",
        new HistogramConfiguration
        {
            Buckets = Histogram.ExponentialBuckets(0.5, 2, 10),
            LabelNames = ["application", "branch", "release"]
        }
    );

    private readonly Counter _downloadInstallerCounter = Metrics.CreateCounter(
        "download_installer_total",
        "Counter of download installer",
        new CounterConfiguration
        {
            LabelNames = ["application", "branch", "release", "installer"]
        }
    );

    private readonly Counter _downloadReleaseCounter = Metrics.CreateCounter(
        "download_release_total",
        "Counter of download release",
        new CounterConfiguration
        {
            LabelNames = ["application", "branch", "release"]
        }
    );

    public void AddDownloadAssets(TimeSpan duration, string application, string branch, Guid release)
    {
        _downloadAssetsHistogram
            .WithLabels(application, branch, release.ToString())
            .Observe(duration.TotalSeconds);
        _downloadReleaseCounter.WithLabels(application, branch, release.ToString())
            .Inc();
    }

    public async Task<T> MeasureDownloadAssets<T>(Func<Task<T>> func, string application, string branch, Guid release)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = await func();
        stopwatch.Stop();
        AddDownloadAssets(stopwatch.Elapsed, application, branch, release);
        return result;
    }

    public async Task MeasureDownloadAssets(Func<Task> func, string application, string branch, Guid release)
    {
        var stopwatch = Stopwatch.StartNew();
        await func();
        stopwatch.Stop();
        AddDownloadAssets(stopwatch.Elapsed, application, branch, release);
    }

    public void AddDownloadInstaller(string application, string branch, Guid release, Guid installer)
    {
        _downloadInstallerCounter
            .WithLabels(application, branch, release.ToString(), installer.ToString())
            .Inc();
        _downloadReleaseCounter
            .WithLabels(application, branch, release.ToString())
            .Inc();
    }
}