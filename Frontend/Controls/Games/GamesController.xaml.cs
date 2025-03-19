using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using Frontend.Models;
using Frontend.Script;

namespace Frontend.Controls.Games;

public partial class GamesController : UserControl
{
    public GamesController()
    {
        InitializeComponent();

        _httpClient.BaseAddress = new Uri(Url.BaseUrl);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SaveRepository.LoadTokenFromFile().AccessToken);
        
        _ = StartGameUpdatesAsync();
    }
    
    private readonly HttpClient _httpClient = new ();
    private Dictionary<string, Game> _games = new ();
    private bool _isGameRunning = true;
    
    private async Task StartGameUpdatesAsync()
    {
        while (_isGameRunning)
        {
            await InitializeGames();
            await Task.Delay(60000);
        }
    }

    private void StopGameUpdates()
    {
        _isGameRunning = false;
    }

    private async Task InitializeGames()
    {
        WrapPanel.Children.Clear();
        _games.Clear();
        
        string url = "Game/games";
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        using var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            var games = JsonSerializer.Deserialize<List<PublicGame>>(result);
            
            if (games != null)
                CreateMenu(games);
        }
        else
            Console.WriteLine($"Ошибка запроса: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
    }
    

    private void CreateMenu(List<PublicGame> games)
    {
        foreach (var game in games)
        {
            Game gameMenu = new Game(game.GameId, game.Title, game.GameMode, game.TotalPlayers, game.PlayerCount, game.IsPotion);
            WrapPanel.Children.Add(gameMenu);
            _games[game.GameId] = gameMenu;
        }
    }
}