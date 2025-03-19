using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using Frontend.Controls;
using Frontend.Controls.Potion;
using Frontend.Controls.Message;
using Frontend.Enums;
using Frontend.Models;
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
        
        _ = GetPlayerDataAsync();
    }

    public delegate void RetryConnect();
    
    private void ClientOnFailedConnect(object? sender, EventArgs e)
    {
        RetryConnect deleteNotify = () =>
        {
            RemoveWarningAfterDelay(0);
            _ = client.ConnectAsync(SaveRepository.LoadTokenFromFile().AccessToken);
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
        CreateGame.IsEnabled = e;
        JoinTheGame.IsEnabled = e;
        GetRandomGame.IsEnabled = e;
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
        CloseMenu closeMenu = () =>
        {
            MainGrid.Children.Remove(_createGameControl);
            _createGameControl = null;
        };

        if (_createGameControl != null)
        {
            closeMenu();
            return;
        }
        
        _createGameControl = new CreateGameControl(mainWindow, this, closeMenu);
        
        Grid.SetRowSpan(_createGameControl, 4);
        Grid.SetColumnSpan(_createGameControl, 4);
        Panel.SetZIndex(_createGameControl, 2);
        MainGrid.Children.Add(_createGameControl);
    }
    
    
    private SetGameIdControl? _setGameIdControl;
    private void JoinTheGame_OnClick(object sender, RoutedEventArgs e)
    {
        CloseMenu closeMenu = () =>
        {
            MainGrid.Children.Remove(_setGameIdControl);
            _setGameIdControl = null;
        };
        
        if (_setGameIdControl != null)
        {
            closeMenu();
            return;
        }
        
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

    private async Task GetPlayerDataAsync()
    {
        await LoadPotion();
        using HttpClient client = new HttpClient();
        
        string url = Url.BaseUrl + "Profile/get-player-data";
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SaveRepository.LoadTokenFromFile().AccessToken);
        
        try
        {
            HttpResponseMessage response = await client.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                PersonData? data = JsonSerializer.Deserialize<PersonData>(responseBody);

                if (data != null)
                {
                    LeagueText.Text = data.League;
                    LevelText.Text = $"Уровень: {data.Level}";
                    ScoreText.Text = data.Score.ToString();
                    
                    ChestControl.StateChest(data.IsChest, data.RequiredNumberOfWinsLevel, data.NumberOfWinsLevel);
                    
                    if (data.Potions != null)
                        InitializePotion(data.Potions);
                }
                else
                    Console.WriteLine($"Не удалось загрузить данные игры");
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode} || {response}");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Request failed: {ex}");
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

    private async Task LoadPotion()
    {
        using HttpClient client = new HttpClient();
        
        string url = Url.BaseUrl + "Potion/get-data-potion";
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);

            using HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            List<PotionEntity>? potions = JsonSerializer.Deserialize<List<PotionEntity>>(result);

            if (potions == null)
                MessageBox.Show("Не удалось загрузить зелья", "Chess-online", MessageBoxButton.OK, MessageBoxImage.Error);
            
            PotionAddFrame(potions);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при выполнении запроса: {ex.Message}");
        }
    }

    private void PotionAddFrame(List<PotionEntity> potions)
    {
        foreach (var potion in potions)
        {
            PotionAddFrame(potion.PotionId, potion.Name, potion.Description, potion.PurchasePrice, potion.EffectType, potion.UnlockLevel);
        }
    }


    private Dictionary<string, PotionControl> _potionControls = new ();
    private void PotionAddFrame(string potionId, string name, string description, int price, PotionType type, int levelUnlock)
    {
        PotionControl control = new PotionControl(potionId, name, description, price, type, levelUnlock);
        PotionStackPanel.Children.Add(control);
        _potionControls.Add(potionId, control);
    }

    private void InitializePotion(List<PotionData> potions)
    {
        foreach (var potion in potions)
        {
            if (_potionControls.ContainsKey(potion.PotionId))
                _potionControls[potion.PotionId].UnlockPotion(potion.Count, potion.IsPurchased, potion.IsUnlocked);
        }
    }

    private void GetRandomGame_OnClick(object sender, RoutedEventArgs e)
    {
        GetRandomGame.IsEnabled = false;
        GetRandomGame.Content = "Загрузка...";
        _ = SendRequestRandomGameAsync();
    }

    private async Task SendRequestRandomGameAsync()
    {
        using HttpClient httpClient = new HttpClient();
        
        string url = Url.BaseUrl + "Game/game";
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", SaveRepository.LoadTokenFromFile().AccessToken);

        using var response = await httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            var gameId = JsonSerializer.Deserialize<string>(result);

            if (gameId != null)
                await SendRequestInGame(gameId);
            else
                MessageBox.Show("Не удалось получить id игры");
        }
        else
            MessageBox.Show($"Ошибка запроса: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
        
        GetRandomGame.IsEnabled = true;
        GetRandomGame.Content = "В БОЙ!";
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

    
    private void ArchiveGame_OnClick(object sender, RoutedEventArgs e)
    {
        mainWindow.OpenGameArchive(this);
    }
}