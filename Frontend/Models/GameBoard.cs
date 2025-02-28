using Frontend.Enums;

namespace Frontend.Models;

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