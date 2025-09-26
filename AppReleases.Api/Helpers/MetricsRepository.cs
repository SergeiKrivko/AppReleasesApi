using System.Text.Json;
using AppReleases.Core.Abstractions;
using AppReleases.Models;

namespace AppReleases.Api.Helpers;

public class MetricsRepository : IMetricsRepository
{
    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri(Environment.GetEnvironmentVariable("PROMETHEUS_URL") ?? ""),
    };

    private readonly string? _prometheusContainer = Environment.GetEnvironmentVariable("PROMETHEUS_CONTAINER");

    public Task<IEnumerable<Metric>> GetMetricsAsync(string query, DateTime timestamp)
    {
        return RequestMetricsAsync($"api/v1/query" +
                                   $"?query={Uri.EscapeDataString(query)}" +
                                   $"&time={Uri.EscapeDataString(((DateTimeOffset)timestamp).ToUnixTimeSeconds().ToString())}");
    }

    public Task<IEnumerable<Metric>> GetMetricsAsync(string query)
    {
        return RequestMetricsAsync($"api/v1/query" +
                                   $"?query={Uri.EscapeDataString(query)}");
    }

    private async Task<IEnumerable<Metric>> RequestMetricsAsync(string url)
    {
        Console.WriteLine(url);
        var response = await _httpClient.GetFromJsonAsync<PrometheusResponse>(url);
        if (response is null)
            throw new Exception("Prometheus response is unprocessable");
        return response.Data.Result
            .Where(m => _prometheusContainer == null || m.Metric["container"] == _prometheusContainer)
            .Select(m => new Metric
            {
                Fields = m.Metric,
                Name = m.Metric.GetValueOrDefault("__name__"),
                Timestamp = DateTime.UnixEpoch + TimeSpan.FromSeconds(m.Value[0].GetDouble()),
                Value = m.Value[1].GetString(),
            });
    }
}