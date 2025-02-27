using Backend.Enums;

namespace Backend.Game.Shapes;

public class Bishop: ChessPiece
{
    public Bishop(string ownerId) : base(PieceType.Bishop, ownerId) { }
    
    public override bool IsValidMove(int newRow, int newCol)
    {
        return true;
    }
}