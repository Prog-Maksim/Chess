namespace Backend.Enums;

public enum PlayerState
{
    /// <summary>
    /// Игрок в игрк
    /// </summary>
    Active,
    
    /// <summary>
    /// Игрок проиграл
    /// </summary>
    Lost,
    
    /// <summary>
    /// У игрока закончилось время
    /// </summary>
    OutOfTime,
    
    /// <summary>
    /// Игрок вышел из игры или отключился
    /// </summary>
    Disconnected
}