namespace Frontend.Models.WebSocketMessage;

/// <summary>
/// События обновления заработанных очков
/// </summary>
public class UpdateScore: BaseWebSocketMessage
{
    public int Score { get; set; }
}