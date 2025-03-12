namespace Backend.Models.Response.WebSocketMessage;

public class AddPerson: BaseWebSocketMessage
{
    public required string PersonId { get; set; }
    public required string Nickname { get; set; }
    public required TimeSpan Time { get; set; }
}