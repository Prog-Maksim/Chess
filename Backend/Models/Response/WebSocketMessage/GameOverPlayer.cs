namespace Backend.Models.Response.WebSocketMessage;

public class GameOverPlayer: BaseWebSocketMessage
{
    public required string PersonId { get; set; }
}