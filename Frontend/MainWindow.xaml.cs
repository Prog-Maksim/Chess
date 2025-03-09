using System.ComponentModel;
using System.Net.Http;
using System.Windows;
using Frontend.Script;
using Frontend.Windows;
using Frontend.Windows.Game;

namespace Frontend;

public partial class MainWindow : Window
{
    private const string Version = "[ALPHA] 01.00.000";
    
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
            CheckApiVersion();
            string token = SaveRepository.ReadToken();
            WebSocketService webSocketService = WebSocketService.Instance;
            _ = webSocketService.ConnectAsync(token);
            
            MainFrame.Content = new MainMenu(webSocketService, this);
        }
    }

    public void OpenMainWindow()
    {
        string token = SaveRepository.ReadToken();
        WebSocketService webSocketService = WebSocketService.Instance;
        _ = webSocketService.ConnectAsync(token);
            
        MainFrame.Content = new MainMenu(webSocketService, this);
    }

    public void OpenGameWindow(string gameId, MainMenu mainMenu, bool create = false)
    {
        MainFrame.Content = new GameMenu(gameId, WebSocketService.Instance, mainMenu, create);
    }

    private void MainWindow_OnClosing(object? sender, CancelEventArgs e)
    {
        WebSocketService webSocketService = WebSocketService.Instance;
        _ = webSocketService.DisconnectAsync();
    }

    private async void CheckApiVersion()
    {
        using HttpClient client = new HttpClient();
        
        string encodedVersion = Uri.EscapeDataString(Version);

        string url = Url.BaseUrl + $"Versioning/check-version?version={encodedVersion}";

        try
        {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            if (!Convert.ToBoolean(responseBody))
            {
                MessageBox.Show("Ваша версия игры устарела. \n\nПожалуйста обновитесь", "Chess-online", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Ошибка запроса: {e.Message}");
        }
    }
}