namespace Frontend.Models.WebSocketMessage;

public class RemainingTimePerson: BaseWebSocketMessage
{
    public TimeSpan Time { get; set; }
    public required string PersonId { get; set; }
}