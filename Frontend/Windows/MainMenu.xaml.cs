using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using Frontend.Controls;
using Frontend.Models;
using Frontend.Models.WebSockerMessage;
using Frontend.Script;

namespace Frontend.Windows;

public partial class MainMenu : Page
{
    public delegate void GetGameId(string gameId);
    
    private MainWindow mainWindow;
    
    public MainMenu()
    {
        InitializeComponent();
    }
    
    public MainMenu(WebSocketService webSocket, MainWindow mainWindow): this()
    {
        this.mainWindow = mainWindow;
        webSocket.OnResultJoinTheGame += StartGame;
    }

    private void CreateGame_OnClick(object sender, RoutedEventArgs e)
    {
        CreateGame.Content = "загрузка";
        CreateGame.IsEnabled = false;
        _ = SendCreateGame();
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
            mainWindow.OpenGameWindow(data.gameId, true);
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
            mainWindow.OpenGameWindow(_gameId);
        }
    }

    private void JoinTheGame_OnClick(object sender, RoutedEventArgs e)
    {
        GetGameId getGameId = GameId;
        SetGameIdControl setGameIdControl = new SetGameIdControl(getGameId);
        Content = setGameIdControl;
    }

    private void GameId(string gameId)
    {
        _gameId = gameId;
        _ = SendRequestInGame(gameId);
    }
    
}