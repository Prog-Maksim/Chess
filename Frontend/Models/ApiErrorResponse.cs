using System.Text.Json.Serialization;

namespace Frontend.Models;

public class ApiErrorResponse
{
    [JsonPropertyName("type")]
    public required string Type { get; set; }

    [JsonPropertyName("title")]
    public required string Title { get; set; }

    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("errors")]
    public Dictionary<string, string[]> Errors { get; set; }

    [JsonPropertyName("traceId")]
    public required string TraceId { get; set; }
}