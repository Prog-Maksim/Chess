using System.Windows;
using Frontend.Script;
using Frontend.Windows;
using Frontend.Windows.Game;

namespace Frontend;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public delegate void OpenMainWindowDelegate();
    
    public MainWindow()
    {
        InitializeComponent();
        
        CheckAuth();
    }

    private void CheckAuth()
    {
        if (!SaveRepository.CheckToken())
        {
            OpenMainWindowDelegate func = OpenMainWindow;
            MainFrame.Content = new Auth(func);
        }
        else
        {
            string token = SaveRepository.ReadToken();
            WebSocketService webSocketService = WebSocketService.Instance;
            _ = webSocketService.ConnectAsync(token);
            
            MainFrame.Content = new MainMenu(webSocketService, this);
        }
    }

    private void OpenMainWindow()
    {
        string token = SaveRepository.ReadToken();
        WebSocketService webSocketService = WebSocketService.Instance;
        _ = webSocketService.ConnectAsync(token);
            
        MainFrame.Content = new MainMenu(webSocketService, this);
    }

    public void OpenGameWindow(string gameId, bool create = false)
    {
        MainFrame.Content = new GameMenu(gameId, WebSocketService.Instance, create);
    }
}