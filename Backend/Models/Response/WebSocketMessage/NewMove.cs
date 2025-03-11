using Backend.Game;

namespace Backend.Models.Response.WebSocketMessage;

public class NewMove: BaseWebSocketMessage
{
    public required Move Move { get; set; }
}