using System.Text.Json.Serialization;

namespace Backend.Models.Response;

public class Token: BaseResponse
{
    /// <summary>
    /// Access токен для доступа к сайту
    /// </summary>
    public required string AccessToken { get; set; }
    
    /// <summary>
    /// Refresh токен обновления
    /// </summary>
    public required string RefreshToken { get; set; }
    
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public required string PersonId { get; set; }
}