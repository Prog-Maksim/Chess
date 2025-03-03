namespace Backend.Enums;

public enum GameState
{
    /// <summary>
    /// Ожидание игроков
    /// </summary>
    WaitingForPlayers,
    /// <summary>
    /// Ожидание начала игры
    /// </summary>
    Countdown,
    /// <summary>
    /// Игра идет
    /// </summary>
    InProgress,
    /// <summary>
    /// Игра завершена
    /// </summary>
    Finished,
    /// <summary>
    /// Игра остановлена
    /// </summary>
    Stopped
}