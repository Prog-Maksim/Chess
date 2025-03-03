using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Frontend.Script;
using Frontend.Windows;

namespace Frontend.Controls;

public partial class SetGameIdControl : UserControl
{
    private readonly MainMenu.GetGameId _gameId;
    private readonly MainMenu.CloseMenu _closeMenu;
    
    public SetGameIdControl(MainMenu.GetGameId gameId, MainMenu.CloseMenu closeMenu)
    {
        InitializeComponent();
        
        _gameId = gameId;
        _closeMenu = closeMenu;

        var client = WebSocketService.Instance;
        Click.IsEnabled = client.IsConnected();
    }

    private void Click_OnClick(object sender, RoutedEventArgs e)
    {
        _gameId(GameIdTextBox.Text);
    }

    private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _closeMenu();
    }
}