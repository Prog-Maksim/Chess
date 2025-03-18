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

        _ = InitializeGames();
    }
    
    private Dictionary<string, Game> _games = new ();

    private async Task InitializeGames()
    {
        HttpClient client = new HttpClient();
        
        string url = Url.BaseUrl + "Game/games";
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", SaveRepository.LoadTokenFromFile().AccessToken);
        
        using var response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            var games = JsonSerializer.Deserialize<List<PublicGame>>(result);
            
            if (games != null)
                CreateMenu(games);
        }
        else
            MessageBox.Show($"Ошибка запроса: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
    }

    private void CreateMenu(List<PublicGame> games)
    {
        foreach (var game in games)
        {
            Game gameMenu = new Game(game.GameId, game.Title, game.GameMode, game.TotalPlayers, game.PlayerCount, game.IsPotion);
            WrapPanel.Children.Add(gameMenu);
            _games[game.GameId] = gameMenu;
            Console.WriteLine(_games.Count);
        }
    }
}