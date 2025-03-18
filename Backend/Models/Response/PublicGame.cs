using Backend.Enums;

namespace Backend.Models.Response;

public class PublicGame
{
    /// <summary>
    /// Название игры
    /// </summary>
    public required string Title { get; set; }
    
    /// <summary>
    /// Идентификатор игры
    /// </summary>
    public required string GameId { get; set; }
    
    /// <summary>
    /// Текущее кол-во игроков
    /// </summary>
    public int PlayerCount { get; set; }
    
    /// <summary>
    /// Кол-во игроков
    /// </summary>
    public int TotalPlayers { get; set; }
    
    /// <summary>
    /// Тип игры
    /// </summary>
    public GameMode GameMode { get; set; }
    
    /// <summary>
    /// Доступны ли зелья
    /// </summary>
    public bool IsPotion { get; set; }
    
}