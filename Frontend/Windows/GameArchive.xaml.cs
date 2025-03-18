using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using Frontend.Controls;
using Frontend.Models;
using Frontend.Script;

namespace Frontend.Windows;

public partial class GameArchive : Page
{
    private readonly MainMenu _mainMenu;
    
    public GameArchive(MainMenu mainMenu)
    {
        InitializeComponent();
        
        _mainMenu = mainMenu;

        _ = InitializeGamesAsync();
    }

    private async Task InitializeGamesAsync()
    {
        using HttpClient client = new HttpClient();
        
        var requestUri = Url.BaseUrl + "Profile/games";

        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", SaveRepository.LoadTokenFromFile().AccessToken);
        using var response = await client.SendAsync(request);
        
        var result = await response.Content.ReadAsStringAsync();
        Console.WriteLine(result);
        var data = JsonSerializer.Deserialize<List<GamesHistory>>(result);

        if (data != null)
            CreateMenu(data);
    }

    private void CreateMenu(List<GamesHistory> games)
    {
        foreach (var game in games)
        {
            GameArchiveMenu menu = new GameArchiveMenu(game.GameName, game.GameDuration, game.PlayerCount, game.GameMode, game.DateCreated, game.IsWinner);
            Menu.Children.Add(menu);
        }
    }

    private void Close_OnClick(object sender, RoutedEventArgs e)
    {
        _mainMenu.OpenMainMenu();
    }
}