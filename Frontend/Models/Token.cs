using System.Text.Json.Serialization;

namespace Frontend.Models;

public class Token: BaseResponse
{
    /// <summary>
    /// Access токен для доступа к сайту
    /// </summary>
    [JsonPropertyName("accessToken")]
    public required string AccessToken { get; set; }
    
    /// <summary>
    /// Refresh токен обновления
    /// </summary>
    [JsonPropertyName("refreshToken")]
    public required string RefreshToken { get; set; }
    
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    [JsonPropertyName("personId")]
    public required string PersonId { get; set; }
}