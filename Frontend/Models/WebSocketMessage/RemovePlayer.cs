namespace Frontend.Models.WebSocketMessage;

public class RemovePlayer: BaseWebSocketMessage
{
    public required string PlayerId { get; set; }
}