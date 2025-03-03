namespace Frontend.Models.WebSockerMessage;

public class FinishGame: BaseWebSocketMessage
{
    public required string Winner { get; set; }
}