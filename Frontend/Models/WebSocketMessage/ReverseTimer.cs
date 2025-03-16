namespace Frontend.Models.WebSocketMessage;

/// <summary>
/// Обратный отсчет до начала игры
/// </summary>
public class ReverseTimer: BaseWebSocketMessage
{
    public int Time { get; set; }
}