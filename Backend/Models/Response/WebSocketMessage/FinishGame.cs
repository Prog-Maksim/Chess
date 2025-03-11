namespace Backend.Models.Response.WebSocketMessage;

public class FinishGame: BaseWebSocketMessage
{
    public required string Winner { get; set; }
}