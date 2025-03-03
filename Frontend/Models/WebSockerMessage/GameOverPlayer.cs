namespace Frontend.Models.WebSockerMessage;

public class GameOverPlayer: BaseWebSocketMessage
{
    public required string PersonId { get; set; }
}