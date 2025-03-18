using System.Text.Json.Serialization;
using Frontend.Enums;

namespace Frontend.Models;

public class GamesHistory
{
    /// <summary>
    /// Имя игры
    /// </summary>
    [JsonPropertyName("gameName")]
    public required string GameName { get; set; }
    
    /// <summary>
    /// Длительность игры
    /// </summary>
    [JsonPropertyName("gameDuration")]
    public TimeSpan GameDuration { get; set; }
    
    /// <summary>
    /// Победил?
    /// </summary>
    [JsonPropertyName("isWinner")]
    public bool IsWinner { get; set; }
    
    /// <summary>
    /// Кол-во игроков
    /// </summary>
    [JsonPropertyName("playerCount")]
    public int PlayerCount { get; set; }
    
    /// <summary>
    /// Тип игры
    /// </summary>
    [JsonPropertyName("gameMode")]
    public GameMode GameMode { get; set; }
    
    /// <summary>
    /// Приватная ли игра
    /// </summary>
    [JsonPropertyName("isPrivate")]
    public bool IsPrivate { get; set; }
    
    /// <summary>
    /// Доступны ли в игре зелья
    /// </summary>
    [JsonPropertyName("isPotion")]
    public bool IsPotion { get; set; }
    
    /// <summary>
    /// Дата и время создания игры
    /// </summary>
    [JsonPropertyName("dateCreated")]
    public DateTime DateCreated { get; set; }
}