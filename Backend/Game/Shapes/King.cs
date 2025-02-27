using Backend.Enums;

namespace Backend.Game.Shapes;

public class King: ChessPiece
{
    public King(string ownerId) : base(PieceType.King, ownerId) { }
    
    public override bool IsValidMove(int newRow, int newCol)
    {
        return true;
    }
}