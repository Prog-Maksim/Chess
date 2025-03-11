namespace Backend.Models.Response.WebSocketMessage;

public class ReversTimeAnActivePlayer: BaseWebSocketMessage
{
    public required string PlayerId { get; set; }
    public TimeSpan Time { get; set; }
}