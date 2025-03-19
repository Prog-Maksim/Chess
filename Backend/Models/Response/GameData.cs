using Backend.Enums;
using Backend.Game;

namespace Backend.Models.Response;

public class GameData
{
    /// <summary>
    /// Идентификатор игрока
    /// </summary>
    public required string PersonId { get; set; }
    
    /// <summary>
    /// Кол-во очков у игрока
    /// </summary>
    public required int Score { get; set; }
    
    /// <summary>
    /// Доступные зелья
    /// </summary>
    public required PotionAvailable PotionAvailable { get; set; }
    
    /// <summary>
    /// Текущие ходы игроков за игру
    /// </summary>
    public required List<Move> Moves { get; set; }
    
    /// <summary>
    /// Кол-во и тип убитых фигур
    /// </summary>
    public required List<PieceType> KillPiece { get; set; }
    
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
    
    /// <summary>
    /// Список пользователей ожидающих вход в игру
    /// </summary>
    public List<GamePlayer>? WaitingPlayers { get; set; }
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
    
    /// <summary>
    /// Цвет игрока
    /// </summary>
    public required string Color { get; set; }
}