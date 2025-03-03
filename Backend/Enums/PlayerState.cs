namespace Backend.Enums;

public enum PlayerState
{
    /// <summary>
    /// Игрок активен
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
    /// Игрок не активен
    /// </summary>
    InActive,
    
    /// <summary>
    /// Игрок вышел из игры или отключился
    /// </summary>
    Disconnected
}