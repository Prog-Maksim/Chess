namespace Frontend.Models.WebSockerMessage;

public class UpdateBoard: BaseWebSocketMessage
{
    public required List<List<GameBoard>> Board { get; set; }
}