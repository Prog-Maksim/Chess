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
    public MainMenu()
    {
        InitializeComponent();
    }
    
    public MainMenu(WebSocketService webSocket): this()
    {
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
            Console.WriteLine($"Ответ от сервера: {responseBody}");
        }
    }

    // Разрешили войти в игру
    private void StartGame(object? sender, ResultJoinTheGame result)
    {
        if (_gameId != null)
        {
            MessageBox.Show("Вам разрешили войти в игру");
            // TODO: Открытие меню игры
        }
    }
}