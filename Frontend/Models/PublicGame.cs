using System.Text.Json.Serialization;
using Frontend.Enums;

namespace Frontend.Models;

public class PublicGame
{
    /// <summary>
    /// Название игры
    /// </summary>
    [JsonPropertyName("title")]
    public required string Title { get; set; }
    
    /// <summary>
    /// Идентификатор игры
    /// </summary>
    [JsonPropertyName("gameId")]
    public required string GameId { get; set; }
    
    /// <summary>
    /// Текущее кол-во игроков
    /// </summary>
    [JsonPropertyName("playerCount")]
    public int PlayerCount { get; set; }
    
    /// <summary>
    /// Кол-во игроков
    /// </summary>
    [JsonPropertyName("totalPlayers")]
    public int TotalPlayers { get; set; }
    
    /// <summary>
    /// Тип игры
    /// </summary>
    [JsonPropertyName("gameMode")]
    public GameMode GameMode { get; set; }
    
    /// <summary>
    /// Доступны ли зелья
    /// </summary>
    [JsonPropertyName("isPotion")]
    public bool IsPotion { get; set; }
}