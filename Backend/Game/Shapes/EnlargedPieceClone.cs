using Backend.Enums;

namespace Backend.Game.Shapes;

public class EnlargedPieceClone: ChessPiece
{
    public ChessPiece Original { get; }
    
    public EnlargedPieceClone(ChessPiece original, int row, int col, PieceType type, string ownerId, int score) : base(type, ownerId, score)
    {
        Original = original;
        Row = row;
        Column = col;
        ChessPieceId = original.ChessPieceId;
    }

    public override bool IsValidMove(int newRow, int newCol)
    {
        return true;
    }
}