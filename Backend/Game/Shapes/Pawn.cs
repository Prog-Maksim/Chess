using Backend.Enums;

namespace Backend.Game.Shapes;

public class Pawn: ChessPiece
{
    public Pawn(string ownerId) : base(PieceType.Pawn, ownerId) { }
    
    public override bool IsValidMove(int newRow, int newCol)
    {
        return true;
    }
}