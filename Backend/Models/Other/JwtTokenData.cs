using Backend.Enums;

namespace Backend.Models.Other;

public class JwtTokenData
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public required string PersonId { get; set; }
    
    /// <summary>
    /// Никнейм пользователя
    /// </summary>
    public string? Nickname { get; set; } 
    
    /// <summary>
    /// Тип токена
    /// </summary>
    public required TokenType TokenType { get; set; }
    
    /// <summary>
    /// Версия пароля
    /// </summary>
    public int Version { get; set; }
    
    /// <summary>
    /// Роль пользователя
    /// </summary>
    public PersonRole Role { get; set; }
    
    /// <summary>
    /// Идентификатор токена
    /// </summary>
    public required string Jti { get; set; }
    
    /// <summary>
    /// Сам токен
    /// </summary>
    public required string Token { get; set; }
}