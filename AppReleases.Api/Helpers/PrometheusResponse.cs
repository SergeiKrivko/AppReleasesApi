using System.Text.Json;

namespace AppReleases.Api.Helpers;

public class PrometheusResponse
{
    public required string Status { get; set; }
    public required Data Data { get; set; }
}

public class Data
{
    public required string ResultType { get; set; }
    public required List<Result> Result { get; set; }
}

public class Result
{
    public required Dictionary<string, string> Metric { get; set; }
    public required List<JsonElement> Value { get; set; }
}