using Backend.Enums;

namespace Backend.Models.Response.WebSocketMessage;

public class KillAllPiece: BaseWebSocketMessage
{
    public List<PieceType>? KillPiece { get; set; }
}