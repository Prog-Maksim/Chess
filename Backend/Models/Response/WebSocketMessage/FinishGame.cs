namespace Backend.Models.Response.WebSocketMessage;

public class FinishGame: BaseWebSocketMessage
{
    public required string WinnerId { get; set; }
    public required bool IsWinner { get; set; }
    public int Score { get; set; }
}