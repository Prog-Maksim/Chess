using Backend.Enums;

namespace Backend.Models.Response.WebSocketMessage;

public class GameStateMessage: BaseWebSocketMessage
{
    public GameState GameState { get; set; }
}