using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Frontend.Controls;
using Frontend.Controls.Message;
using Frontend.Models;
using Frontend.Models.WebSockerMessage;
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
    
    private async void RemoveWarningAfterDelay(int time = 1500)
    {
        await Task.Delay(time);
        Notify.Child = null;
    }

    private void ClientOnIsConnected(object? sender, bool e)
    {
        CoopButton.IsEnabled = e;
        CreateGame.IsEnabled = e;
        JoinTheGame.IsEnabled = e;
    }

    public MainMenu(WebSocketService webSocket, MainWindow mainWindow): this()
    {
        this.mainWindow = mainWindow;
        webSocket.OnResultJoinTheGame += StartGame;
    }

    public delegate void CloseMenu();
    
    private CreateGameControl? CreateGameControl;
    private void CreateGame_OnClick(object sender, RoutedEventArgs e)
    {
        if (CreateGameControl != null)
            return;
        
        CloseMenu closeMenu = () =>
        {
            MainGrid.Children.Remove(CreateGameControl);
            SetGameIdControl = null;
            CoopButton_OnClick(sender, e);
        };
        CreateGameControl = new CreateGameControl(mainWindow, this, closeMenu);
        
        Grid.SetRowSpan(CreateGameControl, 4);
        Grid.SetColumnSpan(CreateGameControl, 4);
        Panel.SetZIndex(CreateGameControl, 2);
        MainGrid.Children.Add(CreateGameControl);
    }
    
    
    private SetGameIdControl? SetGameIdControl;
    private void JoinTheGame_OnClick(object sender, RoutedEventArgs e)
    {
        if (SetGameIdControl != null)
            return;
        
        GetGameId getGameId = GameId;
        CloseMenu closeMenu = () =>
        {
            MainGrid.Children.Remove(SetGameIdControl);
            SetGameIdControl = null;
            CoopButton_OnClick(sender, e);
        };
        SetGameIdControl = new SetGameIdControl(getGameId, closeMenu);
        
        Grid.SetRowSpan(SetGameIdControl, 4);
        Grid.SetColumnSpan(SetGameIdControl, 4);
        Panel.SetZIndex(SetGameIdControl, 2);
        MainGrid.Children.Add(SetGameIdControl);
    }

    private string? _gameId;

    private async Task SendCreateGame()
    {
        HttpClient client = new HttpClient();
        var url = Url.BaseUrl + "Game/create-game";
        
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {SaveRepository.ReadToken()}");
        
        var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await client.PostAsync(url, content);
        
        if (response.IsSuccessStatusCode)
        {
            string responseBody = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<CreateGame>(responseBody);

            Console.WriteLine($"GameId: {data.gameId}");
            mainWindow.OpenGameWindow(data.gameId, this, true);
        }
        else
        {
            MessageBox.Show("Не удалось создать игру!");
        }
        
        CreateGame.Content = "Создать игру";
        CreateGame.IsEnabled = true;
    }
    
    private async Task SendRequestInGame(string gameId)
    {
        HttpClient client = new HttpClient();
        
        var url = Url.BaseUrl + $"Game/login-game?gameId={gameId}";
        
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {SaveRepository.ReadToken()}");
        
        var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
        await client.PostAsync(url, content);
    }
    
    private void StartGame(object? sender, ResultJoinTheGame result)
    {
        if (_gameId != null)
        {
            Console.WriteLine($"GameId: {_gameId}");
            mainWindow.OpenGameWindow(_gameId, this);
        }
    }

    private void GameId(string gameId)
    {
        _gameId = gameId;
        // TODO: Реализовать вывод информации об ошибке
        _ = SendRequestInGame(gameId);
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
}