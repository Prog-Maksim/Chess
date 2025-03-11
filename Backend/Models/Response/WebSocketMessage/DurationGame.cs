namespace Backend.Models.Response.WebSocketMessage;

public class DurationGame: BaseWebSocketMessage
{
    public TimeSpan Time { get; set; }
}