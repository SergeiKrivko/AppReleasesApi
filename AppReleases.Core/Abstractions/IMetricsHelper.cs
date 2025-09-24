namespace AppReleases.Core.Abstractions;

public interface IMetricsHelper
{
    public void AddDownloadAssets(TimeSpan duration, string application, string branch, Guid release);

    public Task<T> MeasureDownloadAssets<T>(Func<Task<T>> func, string application, string branch, Guid release);

    public Task MeasureDownloadAssets(Func<Task> func, string application, string branch, Guid release);

    public void AddDownloadInstaller(string application, string branch, Guid release, Guid installer);
}