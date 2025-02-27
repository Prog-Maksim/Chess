namespace Backend.Models.Response.WebSockerMessage;

public class UpdateBoard: BaseWebSocketMessage
{
    public GameBoard?[,]? Board { get; set; }
}