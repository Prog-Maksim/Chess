namespace Frontend.Models.WebSocketMessage;

public class GameOverPlayer: BaseWebSocketMessage
{
    public required string PersonId { get; set; }
}