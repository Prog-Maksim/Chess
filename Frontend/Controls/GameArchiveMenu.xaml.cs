using System.Windows.Controls;
using System.Windows.Media;
using Frontend.Enums;

namespace Frontend.Controls;

public partial class GameArchiveMenu : UserControl
{
    public GameArchiveMenu()
    {
        InitializeComponent();
    }

    public GameArchiveMenu(string title, TimeSpan durationGame, int players, GameMode type, DateTime createGame, bool isWinner): this()
    {
        Title.Text = title;
        DurationGame.Text = durationGame.ToString();
        RequiredPlayer.Text = players.ToString();
        GameType.Text = type.ToString();
        CreateGame.Text = createGame.ToString("HH:mm:ss dd.MM.yyyy");
        
        if (isWinner)
            MainBorder.Background = (Brush)new BrushConverter().ConvertFrom("#CEFFA0");
        else
            MainBorder.Background = (Brush)new BrushConverter().ConvertFrom("#FFC5C5");
    }
}