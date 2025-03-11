using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Frontend.Controls;

public partial class Moving : UserControl
{
    public class MoveData
    {
        public string Nickname { get; set; }
        public string Move { get; set; }
        public TimeSpan TimeDelta { get; set; }
    }

    private readonly Dictionary<int, char> _chessChar = new ()
    {
        { 1, 'A' },
        { 2, 'B' },
        { 3, 'C' },
        { 4, 'D' },
        { 5, 'E' },
        { 6, 'F' },
        { 7, 'G' },
        { 8, 'H' },
        { 9, 'I' },
        { 10, 'J' },
        { 11, 'K' },
        { 12, 'L' },
        { 13, 'M' },
        { 14, 'N' },
        { 15, 'O' },
        { 16, 'P' },
    };
    private ObservableCollection<MoveData> move = new ();
    
    public Moving()
    {
        InitializeComponent();
    }

    public void AddMove(string nickname, int startRow, int startCol, int endRow, int endCol, TimeSpan timeDelta)
    {
        move.Add(new MoveData { Nickname = nickname, Move = $"{_chessChar[startRow + 1]}{startCol} -> {_chessChar[endRow + 1]}{endCol}", TimeDelta = timeDelta });
        PlayerDataGrid.ItemsSource = move;
    }
}