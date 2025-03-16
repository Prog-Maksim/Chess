namespace Frontend.Models.WebSocketMessage;

/// <summary>
/// Событие проигрыша игрока
/// </summary>
public class GameOverPlayer: BaseWebSocketMessage
{
    public required string PersonId { get; set; }
}