using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Backend.Models.Response;

/// <summary>
/// Базовый класс о результате ответа
/// </summary>
public class BaseResponse
{
    /// <summary>
    /// Описание ответа
    /// </summary>
    public required string Message { get; set; }
    
    /// <summary>
    /// Статус выполнения
    /// </summary>
    [DefaultValue(true)]
    public bool Success { get; set; }
    
    /// <summary>
    /// Статус код ответа
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int StatusCode { get; set; }
    
    /// <summary>
    /// Название ошибки
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Error { get; set; }
}