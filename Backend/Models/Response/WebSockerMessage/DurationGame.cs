namespace Backend.Models.Response.WebSockerMessage;

public class DurationGame: BaseWebSocketMessage
{
    public TimeSpan Time { get; set; }
}