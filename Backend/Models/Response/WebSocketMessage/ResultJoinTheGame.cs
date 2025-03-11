namespace Backend.Models.Response.WebSocketMessage;

/// <summary>
/// Результат ответа, на приглашение в игру
/// </summary>
public class ResultJoinTheGame: BaseWebSocketMessage
{
    public bool Status { get; set; }
    public required string GameId { get; set; }
}