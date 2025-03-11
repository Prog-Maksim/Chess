using Backend.Enums;

namespace Backend.Game.Shapes;

public class Knight: ChessPiece
{
    public Knight(string ownerId) : base(PieceType.Knight, ownerId, 2) { }
    
    public override bool IsValidMove(int newRow, int newCol)
    {
        return true;
    }
}