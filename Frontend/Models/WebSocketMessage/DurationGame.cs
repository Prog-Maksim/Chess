namespace Frontend.Models.WebSocketMessage;

/// <summary>
/// Событие длительности игры
/// </summary>
public class DurationGame: BaseWebSocketMessage
{
    public TimeSpan Time { get; set; }
}