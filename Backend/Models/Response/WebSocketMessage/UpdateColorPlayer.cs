namespace Backend.Models.Response.WebSocketMessage;

public class UpdateColorPlayer: BaseWebSocketMessage
{
    public required string Color { get; set; }
}