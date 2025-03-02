using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Frontend.Controls;
using Frontend.Models;
using Frontend.Models.WebSockerMessage;
using Frontend.Script;

namespace Frontend.Windows.Game;

public partial class GameMenu : Page
{
    private string _gameId;
    private Dictionary<string, PlayerTimeMenu> _players;
    
    public GameMenu()
    {
        InitializeComponent();
        GenerateChessBoard();
    }
    
    public GameMenu(string gameId, WebSocketService webSocket): this()
    {
        _gameId = gameId;
        _players = new Dictionary<string, PlayerTimeMenu>();
        
        webSocket.OnDurationGame += UpdateGameTime;
        webSocket.OnAddPerson += WebSocketOnAddPerson; 
        webSocket.OnRemainingTimePerson += WebSocketOnRemainingTimePerson;
        webSocket.OnUpdateBoard += WebSocketOnUpdateBoard;
        
        _ = GetGameData(gameId);
    }

    public async Task GetGameData(string gameId)
    {
        string url = Url.BaseUrl + "Game/playing-field?gameId=" + gameId;
        
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SaveRepository.ReadToken());

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseContent);
                var gameData = JsonSerializer.Deserialize<GameData>(responseContent);

                AddPersons(gameData.Players);
                UpdateBoard(gameData.Board);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Ошибка HTTP: {e.Message}");
            }
            catch (JsonException e)
            {
                Console.WriteLine($"Ошибка при десериализации JSON: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    private void WebSocketOnUpdateBoard(object? sender, UpdateBoard e)
    {
        UpdateBoard(e.Board);
    }

    private void UpdateBoard(List<List<GameBoard>> board)
    {
        for (int i = 0; i < board.Count; i++)
        {
            for (int j = 0; j < board[i].Count; j++)
            {
                var piece = board[i][j];

                if (piece == null)
                    continue;

                string color = piece.Color;
                var type = piece.Type;

                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string fullPath = Path.Combine(basePath, "Image", "Temporary", color, $"{type}.png");

                Image image = new Image
                {
                    Source = new BitmapImage(new Uri(fullPath, UriKind.Absolute)),
                    Margin = new Thickness(7),
                    Cursor = Cursors.Hand,
                };

                image.MouseLeftButtonDown += (sender, args) =>
                {
                    MessageBox.Show("Нажатие левой кнопки мыши!");
                };
                
                Grid.SetRow(image, i);
                Grid.SetColumn(image, j);
                ChessBoardGrid.Children.Add(image);
            }
        }
    }

    private void WebSocketOnRemainingTimePerson(object? sender, RemainingTimePerson e)
    {
        if (_players.ContainsKey(e.PersonId))
            _players[e.PersonId].UpdateTime(e.Time);
        else
        {
            PlayerTimeMenu playerTimeMenu = new PlayerTimeMenu(e.PersonId, new TimeSpan(hours: 0, minutes: 25, seconds: 0));
            _players[e.PersonId] = playerTimeMenu;
            StackPanelPlayer.Children.Add(playerTimeMenu);
        }
    }

    private void WebSocketOnAddPerson(object? sender, AddPerson e)
    {
        AddPerson(e.PersonId, e.Nickname);
    }

    private void AddPersons(List<GamePlayer> persons)
    {
        foreach (var person in persons)
        {
            AddPerson(person.PlayerId, person.Nickname);
        }
    }

    private void AddPerson(string personId, string nickname)
    {
        if (!_players.ContainsKey(personId))
        {
            PlayerTimeMenu playerTimeMenu = new PlayerTimeMenu(nickname, new TimeSpan(hours: 0, minutes: 25, seconds: 0));
            _players[personId] = playerTimeMenu;
            StackPanelPlayer.Children.Add(playerTimeMenu);
        }
    }

    private void GenerateChessBoard()
    {
        int size = 8;
        
        ChessBoardGrid.Children.Clear();
        
        Brush lightColor = (Brush)new BrushConverter().ConvertFrom("#E0E0E0");
        Brush darkColor = (Brush)new BrushConverter().ConvertFrom("#BEBEBE");
        
        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < size; col++)
            {
                Border cell = new Border
                {
                    Background = (row + col) % 2 == 0 ? lightColor : darkColor,
                    CornerRadius = GetCornerRadius(row, col)
                };
                
                Grid.SetRow(cell, row);
                Grid.SetColumn(cell, col);
                ChessBoardGrid.Children.Add(cell);
            }
        }
    }
    
    private CornerRadius GetCornerRadius(int row, int col)
    {
        int radius = 5;
        
        if (row == 0 && col == 0)
            return new CornerRadius(radius, 0, 0, 0);
        if (row == 7 && col == 0)
            return new CornerRadius(0, 0, 0, radius);
        if (row == 0 && col == 7)
            return new CornerRadius(0, radius, 0, 0);
        if (row == 7 && col == 7)
            return new CornerRadius(0, 0, radius, 0);
    
        return new CornerRadius(0);
    }

    private void UpdateGameTime(object? o, DurationGame game)
    {
        GameTime.Text = game.Time.ToString();
    }
}