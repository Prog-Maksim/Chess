using Backend.Enums;

namespace Backend.Game.Shapes;

public class Queen: ChessPiece
{
    public Queen(string ownerId): base(PieceType.Queen, ownerId) { }
    
    public override bool IsValidMove(int newRow, int newCol)
    {
        return true;
    }
}