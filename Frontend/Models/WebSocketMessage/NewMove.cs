namespace Frontend.Models.WebSocketMessage;

/// <summary>
/// Событие, новых ход игроков
/// </summary>
public class NewMove: BaseWebSocketMessage
{
    public required Move Move { get; set; }
}