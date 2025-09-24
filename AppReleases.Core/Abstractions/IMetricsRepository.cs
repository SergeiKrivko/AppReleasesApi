using AppReleases.Models;

namespace AppReleases.Core.Abstractions;

public interface IMetricsRepository
{
    public Task<IEnumerable<Metric>> GetMetricsAsync(string query, DateTime timestamp);
    public Task<IEnumerable<Metric>> GetMetricsAsync(string query);
}