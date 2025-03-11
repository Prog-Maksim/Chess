namespace Frontend.Models.WebSocketMessage;

public class UpdateBoard: BaseWebSocketMessage
{
    public required List<List<GameBoard>> Board { get; set; }
}