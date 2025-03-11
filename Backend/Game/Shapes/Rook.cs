using Backend.Enums;

namespace Backend.Game.Shapes;

public class Rook: ChessPiece
{
    public Rook(string ownerId) : base(PieceType.Rook, ownerId, 4) { }
    
    public override bool IsValidMove(int newRow, int newCol)
    {
        return true;
    }
}