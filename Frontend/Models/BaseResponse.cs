using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Frontend.Models;

public class BaseResponse
{
    public required string message { get; set; }
    
    [DefaultValue(true)]
    public bool success { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? statusCode { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? error { get; set; }
}