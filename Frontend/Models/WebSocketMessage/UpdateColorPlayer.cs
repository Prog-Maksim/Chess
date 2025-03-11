namespace Frontend.Models.WebSocketMessage;

public class UpdateColorPlayer: BaseWebSocketMessage
{
    public required string Color { get; set; }
}