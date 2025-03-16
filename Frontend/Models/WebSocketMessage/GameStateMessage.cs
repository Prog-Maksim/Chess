using Frontend.Enums;

namespace Frontend.Models.WebSocketMessage;

/// <summary>
/// Событие, новое состояние игры
/// </summary>
public class GameStateMessage: BaseWebSocketMessage
{
    public GameState GameState { get; set; }
}