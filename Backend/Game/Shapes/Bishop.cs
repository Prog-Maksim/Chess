using Backend.Enums;

namespace Backend.Game.Shapes;

public class Bishop: ChessPiece
{
    public int Score { get; private set; }

    public Bishop(string ownerId) : base(PieceType.Bishop, ownerId, 3) { }
    
    public override bool IsValidMove(int newRow, int newCol)
    {
        return true;
    }
}