namespace Frontend.Models.WebSocketMessage;

/// <summary>
/// Событие завершения игры
/// </summary>
public class FinishGame: BaseWebSocketMessage
{
    public required string WinnerId { get; set; }
    public required bool IsWinner { get; set; }
    public int Score { get; set; }
}