namespace Frontend.Models.WebSockerMessage;

public class UpdateBoard: BaseWebSocketMessage
{
    public GameBoard?[,]? Board { get; set; }
}