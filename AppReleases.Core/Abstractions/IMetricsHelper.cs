using AppReleases.Models;

namespace AppReleases.Core.Abstractions;

public interface IMetricsHelper
{
    public void AddDownloadAssets(TimeSpan duration, string application, string branch, Release release);

    public Task<T> MeasureDownloadAssets<T>(Func<Task<T>> func, string application, string branch, Release release);

    public Task MeasureDownloadAssets(Func<Task> func, string application, string branch, Release release);

    public void AddDownloadInstaller(string application, string branch, Release release, Guid installer);
}