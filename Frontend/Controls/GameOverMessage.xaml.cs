using System.Windows;
using System.Windows.Controls;
using Frontend.Windows.Game;

namespace Frontend.Controls;

public partial class GameOverMessage : UserControl
{
    private readonly GameMenu.ExitGame _exitGame;
    private readonly GameMenu.ContinueGame _continueGame;
    
    public GameOverMessage(GameMenu.ExitGame exitGame, GameMenu.ContinueGame continueGame)
    {
        InitializeComponent();

        _exitGame = exitGame;
        _continueGame = continueGame;
    }

    private void Exit_OnClick(object sender, RoutedEventArgs e)
    {
        _exitGame();
    }

    private void Continue_OnClick(object sender, RoutedEventArgs e)
    {
        _continueGame();
    }
}