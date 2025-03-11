using Backend.Enums;

namespace Backend.Game.Shapes;

public class Pawn: ChessPiece
{
    public Pawn(string ownerId) : base(PieceType.Pawn, ownerId, 1) { }
    
    public bool IsSecondMove { get; set; } = false;
    
    public override bool IsValidMove(int newRow, int newCol)
    {
        return true;
    }
}