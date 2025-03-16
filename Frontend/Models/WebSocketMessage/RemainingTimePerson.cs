namespace Frontend.Models.WebSocketMessage;

/// <summary>
/// Оставшееся время игры у пользователя
/// </summary>
public class RemainingTimePerson: BaseWebSocketMessage
{
    public TimeSpan Time { get; set; }
    public required string PersonId { get; set; }
}