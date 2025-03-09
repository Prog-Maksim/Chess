namespace Backend.Models.Response.WebSockerMessage;

public class UpdateColorPlayer: BaseWebSocketMessage
{
    public required string Color { get; set; }
}