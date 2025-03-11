namespace Backend.Models.Response.WebSocketMessage;

public class UpdateScore: BaseWebSocketMessage
{
    public int Score { get; set; }
}