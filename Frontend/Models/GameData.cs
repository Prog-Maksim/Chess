using System.Text.Json.Serialization;
using Frontend.Enums;

namespace Frontend.Models;

public class GameData
{
    /// <summary>
    /// Идентификатор игрока
    /// </summary>
    public string PersonId { get; set; }
    
    /// <summary>
    /// Доступные зелья
    /// </summary>
    public required PotionAvailable PotionAvailable { get; set; }
    
    /// <summary>
    /// Идентификатор игры
    /// </summary>
    public string GameId { get; set; }

    /// <summary>
    /// Название игры
    /// </summary>
    public string GameName { get; set; }
    
    /// <summary>
    /// Игровое поле
    /// </summary>
    public required List<List<GameBoard?>> Board { get; set; }
    
    /// <summary>
    /// Список игроков
    /// </summary>
    public List<GamePlayer> Players { get; set; }
    
    /// <summary>
    /// Текущий играющий игрок
    /// </summary>
    public string CurrentPlayer { get; set; }
    
    /// <summary>
    /// Состояние игры
    /// </summary>
    public GameState GameState { get; set; }
}

public class GameBoardData
{
    /// <summary>
    /// Цвет фигуры
    /// </summary>
    [JsonPropertyName("color")]
    public required string Color { get; set; }
    /// <summary>
    /// Тип фигуры
    /// </summary>
    [JsonPropertyName("type")]
    public required PieceType Type { get; set; }
    /// <summary>
    /// Идентификатор фигуры
    /// </summary>
    [JsonPropertyName("pieceId")]
    public required string PieceId { get; set; }
    /// <summary>
    /// Кому принадлежит фигура
    /// </summary>
    [JsonPropertyName("personId")]
    public required string PersonId { get; set; }
}

public class GamePlayer
{
    /// <summary>
    /// Идентификатор игрока
    /// </summary>
    public string PlayerId { get; set; }
    
    /// <summary>
    /// Никнейм игрока
    /// </summary>
    public string Nickname { get; set; }
    
    /// <summary>
    /// Время, которое осталось у игрока на игру
    /// </summary>
    public TimeSpan Time { get; set; }
    
    /// <summary>
    /// Цвет игрока
    /// </summary>
    public required string Color { get; set; }
}