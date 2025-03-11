namespace Frontend.Models;

public class Move
{
    /// <summary>
    /// Идентификатор хода
    /// </summary>
    public required string MoveId { get; set; }
    
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public required string PlayerId { get; set; }
    
    /// <summary>
    /// Дата и время хода
    /// </summary>
    public DateTime DataMove { get; set; }
    
    /// <summary>
    /// Длительность хода
    /// </summary>
    public TimeSpan Duration { get; set; }
    
    /// <summary>
    /// Начальный ряд фигуры
    /// </summary>
    public int StartRow { get; set; }
    
    /// <summary>
    /// Начальная строка фигуры
    /// </summary>
    public int StartColumn { get; set; }
    
    /// <summary>
    /// Конечный ряд фигуры
    /// </summary>
    public int EndRow { get; set; }
    
    /// <summary>
    /// Конечная строка фигуры
    /// </summary>
    public int EndColumn { get; set; }
}