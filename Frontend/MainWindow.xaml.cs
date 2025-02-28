using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Frontend.Scrypt;
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
            
            MainFrame.Content = new MainMenu(webSocketService);
        }
    }

    private void OpenMainWindow()
    {
        string token = SaveRepository.ReadToken();
        WebSocketService webSocketService = WebSocketService.Instance;
        _ = webSocketService.ConnectAsync(token);
            
        MainFrame.Content = new MainMenu(webSocketService);
    }
}