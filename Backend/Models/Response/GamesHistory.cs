using Backend.Enums;

namespace Backend.Models.Response;

public class GamesHistory
{
    /// <summary>
    /// Имя игры
    /// </summary>
    public required string GameName { get; set; }
    
    /// <summary>
    /// Длительность игры
    /// </summary>
    public TimeSpan GameDuration { get; set; }
    
    /// <summary>
    /// Победил?
    /// </summary>
    public bool IsWinner { get; set; }
    
    /// <summary>
    /// Кол-во игроков
    /// </summary>
    public int PlayerCount { get; set; }
    
    /// <summary>
    /// Тип игры
    /// </summary>
    public GameMode GameMode { get; set; }
    
    /// <summary>
    /// Приватная ли игра
    /// </summary>
    public bool IsPrivate { get; set; }
    
    /// <summary>
    /// Доступны ли в игре зелья
    /// </summary>
    public bool IsPotion { get; set; }
    
    /// <summary>
    /// Дата и время создания игры
    /// </summary>
    public DateTime DateCreated { get; set; }
}