using Backend.Enums;

namespace Backend.Models.Response;

public class GameData
{
    /// <summary>
    /// Идентификатор игрока
    /// </summary>
    public required string PersonId { get; set; }
    
    /// <summary>
    /// Идентификатор игры
    /// </summary>
    public required string GameId { get; set; }
    /// <summary>
    /// Название игры
    /// </summary>
    public required string GameName { get; set; }
    
    /// <summary>
    /// Игровое поле
    /// </summary>
    public List<List<GameBoard?>> Board { get; set; }
    /// <summary>
    /// Список игроков
    /// </summary>
    public List<GamePlayer>? Players { get; set; }
    
    /// <summary>
    /// Текущий играющий игрок
    /// </summary>
    public required string CurrentPlayer { get; set; }
    /// <summary>
    /// Состояние игры
    /// </summary>
    public required GameState GameState { get; set; }
}

public class GameBoard
{
    /// <summary>
    /// Цвет фигуры
    /// </summary>
    public required string Color { get; set; }
    /// <summary>
    /// Тип фигуры
    /// </summary>
    public required PieceType Type { get; set; }
    /// <summary>
    /// Идентификатор фигуры
    /// </summary>
    public required string PieceId { get; set; }
    /// <summary>
    /// Кому принадлежит фигура
    /// </summary>
    public required string PersonId { get; set; }
}

public class GamePlayer
{
    /// <summary>
    /// Идентификатор игрока
    /// </summary>
    public required string PlayerId { get; set; }
    /// <summary>
    /// Никнейм игрока
    /// </summary>
    public required string Nickname { get; set; }
    /// <summary>
    /// Время, которое осталось у игрока на игру
    /// </summary>
    public TimeSpan Time { get; set; }
}