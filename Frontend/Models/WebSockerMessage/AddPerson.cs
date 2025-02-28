namespace Frontend.Models.WebSockerMessage;

public class AddPerson: BaseWebSocketMessage
{
    public required string personId { get; set; }
    public required string nickname { get; set; }
}