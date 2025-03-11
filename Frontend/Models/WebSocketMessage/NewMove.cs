namespace Frontend.Models.WebSocketMessage;

public class NewMove: BaseWebSocketMessage
{
    public required Move Move { get; set; }
}