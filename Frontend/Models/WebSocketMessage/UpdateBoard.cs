namespace Frontend.Models.WebSocketMessage;

/// <summary>
/// Событие обновление игрового поля
/// </summary>
public class UpdateBoard: BaseWebSocketMessage
{
    public required List<List<GameBoard>> Board { get; set; }
}