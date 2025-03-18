using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Frontend.Enums;
using Frontend.Script;

namespace Frontend.Controls.Games;

public partial class Game : UserControl
{
    private string _gameId;
    
    public Game()
    {
        InitializeComponent();
    }
    
    public Game(string gameId, string title, GameMode type, int requiredPlayers, int currentPlayers, bool isPotion): this()
    {
        _gameId = gameId;
        
        TitleGame.Text = title;
        string potions = isPotion ? "с зельями" : "без зелий";
        DescriptionGame.Text = $"Тип: {type}; игроков: {requiredPlayers}; {potions}";
        
        CurrentPlayerGame.Text = $"Игроков: {currentPlayers}";
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        _ = SendRequestInGame(_gameId);
    }
    
    private async Task SendRequestInGame(string gameId)
    {
        HttpClient httpClient = new HttpClient();
        
        var url = Url.BaseUrl + $"Game/login-game?gameId={gameId}";
        
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {SaveRepository.LoadTokenFromFile().AccessToken}");
        
        var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
        await httpClient.PostAsync(url, content);
    }
}