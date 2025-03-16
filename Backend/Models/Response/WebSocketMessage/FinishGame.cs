namespace Backend.Models.Response.WebSocketMessage;

public class FinishGame: BaseWebSocketMessage
{
    public TimeSpan DurationGame { get; set; }
}