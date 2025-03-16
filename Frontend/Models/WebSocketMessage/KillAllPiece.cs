using Frontend.Enums;

namespace Frontend.Models.WebSocketMessage;

/// <summary>
/// Событие обновления кол-ва убитых фигур
/// </summary>
public class KillAllPiece: BaseWebSocketMessage
{
    public List<PieceType>? KillPiece { get; set; }
}