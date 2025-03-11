using Backend.Game.Shapes;

namespace Backend.Models.Response.WebSocketMessage;

public class KillAllPiece: BaseWebSocketMessage
{
    public List<ChessPiece> KillPiece { get; set; }
}