namespace Frontend.Models.WebSocketMessage;
public class BaseWebSocketMessage
{
    public required string MessageType { get; set; }
    public int StatusCode { get; set; }
    public bool Success { get; set; }
    public required string Message { get; set; }
}