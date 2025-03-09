namespace Backend.Models.Response.WebSockerMessage;

public class RemovePlayer: BaseWebSocketMessage
{
    public required string PlayerId { get; set; }
}