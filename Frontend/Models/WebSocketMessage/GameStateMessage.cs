using Frontend.Enums;

namespace Frontend.Models.WebSocketMessage;

public class GameStateMessage: BaseWebSocketMessage
{
    public GameState GameState { get; set; }
}