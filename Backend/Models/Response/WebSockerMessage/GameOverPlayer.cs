namespace Backend.Models.Response.WebSockerMessage;

public class GameOverPlayer: BaseWebSocketMessage
{
    public required string PersonId { get; set; }
}