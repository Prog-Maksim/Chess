namespace Frontend.Models.WebSocketMessage;

/// <summary>
/// Событие завершения игры
/// </summary>
public class FinishGame: BaseWebSocketMessage
{
    public TimeSpan DurationGame { get; set; }
}