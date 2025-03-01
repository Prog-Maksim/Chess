namespace Frontend.Models.WebSockerMessage;

public class AddPerson: BaseWebSocketMessage
{
    public required string PersonId { get; set; }
    public required string Nickname { get; set; }
}