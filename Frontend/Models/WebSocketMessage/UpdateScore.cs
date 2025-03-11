namespace Frontend.Models.WebSocketMessage;

public class UpdateScore: BaseWebSocketMessage
{
    public int Score { get; set; }
}