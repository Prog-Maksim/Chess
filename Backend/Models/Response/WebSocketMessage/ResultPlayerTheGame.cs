using System.Text.Json.Serialization;
using Backend.Enums;

namespace Backend.Models.Response.WebSocketMessage;

public class ResultPlayerTheGame: BaseWebSocketMessage
{
    /// <summary>
    /// Статус игрока (победил, не победил)
    /// </summary>
    public bool Status { get; set; }
    
    /// <summary>
    /// Название лиги
    /// </summary>
    public required string League { get; set; }
    
    /// <summary>
    /// Заработанные очки
    /// </summary>
    public required ScoreData ScoreData { get; set; }
    
    /// <summary>
    /// Начисление рейтинга за игру
    /// </summary>
    public int Rating { get; set; }
    
    /// <summary>
    /// Бонус, зелье за игру
    /// </summary>
    public AddPotion? AddPotion { get; set; }
    
    /// <summary>
    /// Использованные зелья за игру
    /// </summary>
    public List<PotionType>? UsedPotions { get; set; }
}

public class ScoreData
{
    /// <summary>
    /// Очки за игру
    /// </summary>
    public int Score { get; set; }
    
    /// <summary>
    /// Очки за победу
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? AddScoreWine { get; set; }
    
    /// <summary>
    /// Очки за неиспользуемые зелья
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? PotionScore { get; set; }
    
    /// <summary>
    /// Очки за режим игры и кол-во игроков
    /// </summary>
    public int ModeScore { get; set; }
    
    /// <summary>
    /// Итого заработанных очков за игру
    /// </summary>
    public int TotalScore { get; set; }
}

public class AddPotion
{
    /// <summary>
    /// Тип зелья
    /// </summary>
    public PotionType Type { get; set; }
    
    /// <summary>
    /// Название зелья
    /// </summary>
    public required string PotionName { get; set; }
    
    /// <summary>
    /// Кол-во зелье
    /// </summary>
    public int Count { get; set; }
}