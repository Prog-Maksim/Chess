namespace Frontend.Models.WebSocketMessage;

/// <summary>
/// Время, которое осталось у игрока на перезаход в игру
/// </summary>
public class ReversTimeAnActivePlayer: BaseWebSocketMessage
{
    public required string PlayerId { get; set; }
    public TimeSpan Time { get; set; }
}