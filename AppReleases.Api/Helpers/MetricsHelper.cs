using System.Diagnostics;
using AppReleases.Core.Abstractions;
using AppReleases.Models;
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
            LabelNames = ["application", "branch", "release", "platform", "version"]
        }
    );

    private readonly Counter _downloadBundleCounter = Metrics.CreateCounter(
        "download_bundle_total",
        "Counter of download bundle",
        new CounterConfiguration
        {
            LabelNames = ["application", "branch", "release", "platform", "version", "bundle"]
        }
    );

    private readonly Counter _downloadReleaseCounter = Metrics.CreateCounter(
        "download_release_total",
        "Counter of download release",
        new CounterConfiguration
        {
            LabelNames = ["application", "branch", "release", "platform", "version"]
        }
    );

    public void AddDownloadAssets(TimeSpan duration, string application, string branch, Release release)
    {
        _downloadAssetsHistogram
            .WithLabels(application, branch, release.Id.ToString(), release.Platform ?? "", release.Version.ToString())
            .Observe(duration.TotalSeconds);
        _downloadReleaseCounter
            .WithLabels(application, branch, release.Id.ToString(), release.Platform ?? "", release.Version.ToString())
            .Inc();
    }

    public async Task<T> MeasureDownloadAssets<T>(Func<Task<T>> func, string application, string branch,
        Release release)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = await func();
        stopwatch.Stop();
        AddDownloadAssets(stopwatch.Elapsed, application, branch, release);
        return result;
    }

    public async Task MeasureDownloadAssets(Func<Task> func, string application, string branch, Release release)
    {
        var stopwatch = Stopwatch.StartNew();
        await func();
        stopwatch.Stop();
        AddDownloadAssets(stopwatch.Elapsed, application, branch, release);
    }

    public void AddDownloadBundle(string application, string branch, Release release, Guid bundle)
    {
        _downloadBundleCounter
            .WithLabels(application, branch, release.Id.ToString(), release.Platform ?? "", release.Version.ToString(),
                bundle.ToString())
            .Inc();
        _downloadReleaseCounter
            .WithLabels(application, branch, release.Id.ToString(), release.Platform ?? "", release.Version.ToString())
            .Inc();
    }

    public void PublishDownloadRelease(string application, string branch, Release release)
    {
        _downloadReleaseCounter
            .WithLabels(application, branch, release.Id.ToString(), release.Platform ?? "", release.Version.ToString())
            .Publish();
    }

    public void PublishDownloadAssets(string application, string branch, Release release)
    {
        _downloadAssetsHistogram
            .WithLabels(application, branch, release.Id.ToString(), release.Platform ?? "", release.Version.ToString())
            .Publish();
    }

    public void PublishDownloadBundle(string application, string branch, Release release, Guid bundle)
    {
        _downloadBundleCounter
            .WithLabels(application, branch, release.Id.ToString(), release.Platform ?? "", release.Version.ToString(),
                bundle.ToString())
            .Publish();
    }
}