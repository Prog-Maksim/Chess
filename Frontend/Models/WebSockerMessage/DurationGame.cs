namespace Frontend.Models.WebSockerMessage;

public class DurationGame: BaseWebSocketMessage
{
    public TimeSpan Time { get; set; }
}