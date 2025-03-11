using System.Windows.Controls;
using Frontend.Enums;

namespace Frontend.Controls;

public partial class KillPieceControl : UserControl
{
    public KillPieceControl()
    {
        InitializeComponent();
    }

    public void UpdateKillPiece(PieceType type, int num)
    {
        if (type == PieceType.King)
            KingText.Text = num.ToString();
        else if (type == PieceType.Queen)
            QueenText.Text = num.ToString();
        else if (type == PieceType.Rook)
            RookText.Text = num.ToString();
        else if (type == PieceType.Knight)
            KnightText.Text = num.ToString();
        else if (type == PieceType.Bishop)
            BishopText.Text = num.ToString();
        else if (type == PieceType.Pawn)
            PawnText.Text = num.ToString();
    }
}