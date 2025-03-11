using Frontend.Enums;

namespace Frontend.Models.WebSocketMessage;

public class KillAllPiece: BaseWebSocketMessage
{
    public List<PieceType>? KillPiece { get; set; }
}