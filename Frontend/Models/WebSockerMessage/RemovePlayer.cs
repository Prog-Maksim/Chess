namespace Frontend.Models.WebSockerMessage;

public class RemovePlayer: BaseWebSocketMessage
{
    public required string PlayerId { get; set; }
}