namespace Frontend.Models.WebSocketMessage;

/// <summary>
/// У игрока изменился цвет
/// </summary>
public class UpdateColorPlayer: BaseWebSocketMessage
{
    public required string Color { get; set; }
}