using AppReleases.Models;

namespace AppReleases.Core.Abstractions;

public interface IMetricsHelper
{
    public void AddDownloadAssets(TimeSpan duration, string application, string branch, Release release);

    public Task<T> MeasureDownloadAssets<T>(Func<Task<T>> func, string application, string branch, Release release);

    public Task MeasureDownloadAssets(Func<Task> func, string application, string branch, Release release);

    public void AddDownloadBundle(string application, string branch, Release release, Guid bundle);
    public void AddDownloadInstaller(string application, string branch, Release release, Guid builder);
    public void PublishDownloadRelease(string application, string branch, Release release);
    public void PublishDownloadAssets(string application, string branch, Release release);
    public void PublishDownloadBundle(string application, string branch, Release release, Guid bundle);
}