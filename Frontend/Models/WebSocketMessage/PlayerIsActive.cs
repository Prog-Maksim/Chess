namespace Frontend.Models.WebSocketMessage;

/// <summary>
/// Событие статуса пользователя (активен не активен)
/// </summary>
public class PlayerIsActive: BaseWebSocketMessage
{
    public required string PlayerId { get; set; }
    public required string Nickname { get; set; }
    public bool State { get; set; }
    public TimeSpan Time { get; set; }
}