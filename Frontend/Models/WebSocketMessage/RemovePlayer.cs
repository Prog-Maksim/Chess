namespace Frontend.Models.WebSocketMessage;

/// <summary>
/// Игрок был удален из игры или выбыл
/// </summary>
public class RemovePlayer: BaseWebSocketMessage
{
    public required string PlayerId { get; set; }
}