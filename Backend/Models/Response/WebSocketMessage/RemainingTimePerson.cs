namespace Backend.Models.Response.WebSocketMessage;

public class RemainingTimePerson: BaseWebSocketMessage
{
    public TimeSpan Time { get; set; }
    public required string PersonId { get; set; }
}