using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Frontend.Controls;
using Frontend.Controls.Message;
using Frontend.Models.WebSocketMessage;
using Frontend.Script;

namespace Frontend.Windows;

public partial class MainMenu : Page
{
    public delegate void GetGameId(string gameId);

    private readonly WebSocketService client;
    
    private MainWindow mainWindow;
    
    public MainMenu()
    {
        InitializeComponent();
        
        client = WebSocketService.Instance;
        client.OnIsConnected += ClientOnIsConnected;
        client.OnConnectRetry += ClientOnConnectRetry;
        client.OnFailedConnect += ClientOnFailedConnect;

        GetScoreAsync();
    }

    public delegate void RetryConnect();
    
    private void ClientOnFailedConnect(object? sender, EventArgs e)
    {
        RetryConnect deleteNotify = () =>
        {
            RemoveWarningAfterDelay(0);
            _ = client.ConnectAsync(SaveRepository.ReadToken());
        };
        FailedMessageConnect warningRetryConnect = new FailedMessageConnect(deleteNotify);
        warningRetryConnect.Margin = new Thickness(0, 10, 0, 35);
        Notify.Child = warningRetryConnect;
    }

    private void ClientOnConnectRetry(object? sender, EventArgs e)
    {
        RemoveWarningAfterDelay(0);
        
        WarningRetryConnect warningRetryConnect = new WarningRetryConnect();
        warningRetryConnect.Margin = new Thickness(0, 10, 0, 35);
        Notify.Child = warningRetryConnect;
        
        RemoveWarningAfterDelay();
    }
    
    private void ClientOnIsConnected(object? sender, bool e)
    {
        CoopButton.IsEnabled = e;
        CreateGame.IsEnabled = e;
        JoinTheGame.IsEnabled = e;
    }
    
    private async void RemoveWarningAfterDelay(int time = 1500)
    {
        await Task.Delay(time);
        Notify.Child = null;
    }

    public MainMenu(WebSocketService webSocket, MainWindow mainWindow): this()
    {
        this.mainWindow = mainWindow;
        webSocket.OnResultJoinTheGame += StartGame;
    }

    public delegate void CloseMenu();
    
    private CreateGameControl? _createGameControl;
    private void CreateGame_OnClick(object sender, RoutedEventArgs e)
    {
        if (_createGameControl != null)
            return;
        
        CloseMenu closeMenu = () =>
        {
            MainGrid.Children.Remove(_createGameControl);
            _createGameControl = null;
            CoopButton_OnClick(sender, e);
        };
        _createGameControl = new CreateGameControl(mainWindow, this, closeMenu);
        
        Grid.SetRowSpan(_createGameControl, 4);
        Grid.SetColumnSpan(_createGameControl, 4);
        Panel.SetZIndex(_createGameControl, 2);
        MainGrid.Children.Add(_createGameControl);
    }
    
    
    private SetGameIdControl? _setGameIdControl;
    private void JoinTheGame_OnClick(object sender, RoutedEventArgs e)
    {
        if (_setGameIdControl != null)
            return;
        
        CloseMenu closeMenu = () =>
        {
            MainGrid.Children.Remove(_setGameIdControl);
            _setGameIdControl = null;
            CoopButton_OnClick(sender, e);
        };
        _setGameIdControl = new SetGameIdControl(closeMenu);
        
        Grid.SetRowSpan(_setGameIdControl, 4);
        Grid.SetColumnSpan(_setGameIdControl, 4);
        Panel.SetZIndex(_setGameIdControl, 2);
        MainGrid.Children.Add(_setGameIdControl);
    }
    
    private void StartGame(object? sender, ResultJoinTheGame result)
    {
        if (result.Status)
            mainWindow.OpenGameWindow(result.GameId, this);
        else
        {
            if (_setGameIdControl != null)
                _setGameIdControl.SetTextError("Вам запретили войти в игру", true);
        }
        
        if (_setGameIdControl != null)
            _setGameIdControl.SetButtonEnabled();
    }

    public void OpenMainMenu()
    {
        mainWindow.OpenMainWindow();
    }

    private bool _buttonIsOpen;
    private void CoopButton_OnClick(object sender, RoutedEventArgs e)
    {
        CoopPopup.IsOpen = !_buttonIsOpen;
        _buttonIsOpen = !_buttonIsOpen;
        
        if (_buttonIsOpen)
            CoopButton.Background = (Brush)new BrushConverter().ConvertFrom("#5053A7");
        else
            CoopButton.Background = (Brush)new BrushConverter().ConvertFrom("#7074D5");
    }

    private async void GetScoreAsync()
    {
        using HttpClient client = new HttpClient();
        
        string url = Url.BaseUrl + "Profile/get-score";
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SaveRepository.ReadToken());
        
        try
        {
            HttpResponseMessage response = await client.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                ScoreText.Text = responseBody;
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Request failed: {ex.Message}");
        }
    }

    private Settings? _settings;
    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        CloseMenu closeMenu = () =>
        {
            MainGrid.Children.Remove(_settings);
            _settings = null;
        };
        
        if (_settings == null)
        {
            _settings = new Settings(closeMenu);
            
            _settings.VerticalAlignment = VerticalAlignment.Top;
            _settings.HorizontalAlignment = HorizontalAlignment.Right;
            _settings.Margin = new Thickness(0, 60,20, 100);
            
            Grid.SetRowSpan(_settings, 4);
            Grid.SetColumnSpan(_settings, 4);
            Panel.SetZIndex(_settings, 2);
            MainGrid.Children.Add(_settings);
        }
        else
        {
            closeMenu();
        }
    }
}