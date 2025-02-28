namespace Frontend.Models.WebSockerMessage;

/// <summary>
/// Заявка на вступление в игру
/// </summary>
public class JoinTheGame: BaseWebSocketMessage
{
    public required string nickname { get; set; }
    public required string personId { get; set; }
}