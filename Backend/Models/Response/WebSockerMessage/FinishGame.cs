namespace Backend.Models.Response.WebSockerMessage;

public class FinishGame: BaseWebSocketMessage
{
    public required string Winner { get; set; }
}