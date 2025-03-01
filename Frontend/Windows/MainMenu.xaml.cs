using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using Frontend.Models;
using Frontend.Models.WebSockerMessage;
using Frontend.Scrypt;

namespace Frontend.Windows;

public partial class MainMenu : Page
{
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

            _ = SendRequestInGame(data.gameId);
            _gameId = data.gameId;
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
        HttpResponseMessage response = await client.PostAsync(url, content);
        
        if (response.IsSuccessStatusCode)
        {
            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Result to server: {responseBody}");
        }
    }

    // Разрешили войти в игру
    private void StartGame(object? sender, ResultJoinTheGame result)
    {
        Console.WriteLine("Успешно!");
        
        if (_gameId != null)
        {
            Console.WriteLine($"GameId: {_gameId}");
            mainWindow.OpenGameWindow(_gameId);
            // TODO: Открытие меню игры
        }
    }
}