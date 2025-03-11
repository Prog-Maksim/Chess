namespace Backend.Models.Response.WebSocketMessage;

public class UpdateBoard: BaseWebSocketMessage
{
    public GameBoard?[,]? Board { get; set; }
}