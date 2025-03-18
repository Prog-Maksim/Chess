namespace Frontend.Models;

public class Token: BaseResponse
{
    /// <summary>
    /// Access токен для доступа к сайту
    /// </summary>
    public required string accessToken { get; set; }
    
    /// <summary>
    /// Refresh токен обновления
    /// </summary>
    public required string refreshToken { get; set; }
    
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public required string personId { get; set; }
}