using System.Windows;
using System.Windows.Controls;
using Frontend.Windows;

namespace Frontend.Controls;

public partial class SetGameIdControl : UserControl
{
    private readonly MainMenu.GetGameId _gameId;
    
    public SetGameIdControl(MainMenu.GetGameId gameId)
    {
        InitializeComponent();
        
        _gameId = gameId;
    }

    private void Click_OnClick(object sender, RoutedEventArgs e)
    {
        _gameId(GameIdTextBox.Text);
    }
}