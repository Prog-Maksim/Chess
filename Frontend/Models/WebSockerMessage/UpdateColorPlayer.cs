namespace Frontend.Models.WebSockerMessage;

public class UpdateColorPlayer: BaseWebSocketMessage
{
    public required string Color { get; set; }
}