namespace Frontend.Models.WebSocketMessage;

/// <summary>
/// Событие о добавление нового пользователя
/// </summary>
public class AddPerson: BaseWebSocketMessage
{
    public required string PersonId { get; set; }
    public required string Nickname { get; set; }
    public required TimeSpan Time { get; set; }
}