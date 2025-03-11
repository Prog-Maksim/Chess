namespace Frontend.Models.WebSocketMessage;

public class DurationGame: BaseWebSocketMessage
{
    public TimeSpan Time { get; set; }
}