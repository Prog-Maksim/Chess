using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Frontend.Controls;
using Frontend.Enums;
using Frontend.Models;
using Frontend.Models.Request;
using Frontend.Models.WebSockerMessage;
using Frontend.Script;

namespace Frontend.Windows.Game;

public partial class GameMenu : Page
{
    // состояние игры
    private bool _gameState;
    // состояние игрока, может ли он походить
    private bool _playerTurn;
    // идентификатор карты
    private string _gameId;
    // меню игроков
    private Dictionary<string, PlayerTimeMenu> _players;
    // идентификатор игрока чей щяс ход
    private string _playerIdTern;
    // размер поля
    private int _gameSize;
    
    
    private readonly WebSocketService _webSocketService;
    private GameIdControl? _gameIdControl;
    private readonly MainMenu _mainMenu;

    public delegate void ExitGame();
    public delegate void ContinueGame();


    public GameMenu()
    {
        InitializeComponent();
    }
    
    public GameMenu(string gameId, WebSocketService webSocket, MainMenu mainMenu, bool create = false): this()
    {
        _gameId = gameId;
        _players = new Dictionary<string, PlayerTimeMenu>();
        _mainMenu = mainMenu;
        _webSocketService = webSocket;
        
        webSocket.OnJoinTheGame += WebSocketOnJoinTheGame;
        webSocket.OnDurationGame += UpdateGameTime;
        webSocket.OnAddPerson += WebSocketOnAddPerson; 
        webSocket.OnRemainingTimePerson += WebSocketOnRemainingTimePerson;
        webSocket.OnUpdateBoard += WebSocketOnUpdateBoard;
        webSocket.OnReverseTimer += WebSocketOnReverseTimer;
        webSocket.OnGameOverPlayer += WebSocketOnGameOverPlayer;
        webSocket.OnIsActivePlayer += WebSocketOnIsActivePlayer;
        webSocket.OnReverseTimeAnActivePlayer += WebSocketOnReverseTimeAnActivePlayer;
        webSocket.OnGameFinished += WebSocketOnGameFinished;
        webSocket.OnRemovePlayer += WebSocketOnRemovePlayer;
        webSocket.OnUpdateColor += WebSocketOnUpdateColor;
        
        _ = GetGameData();
        WaitingGame(create);
    }

    private void WaitingGame(bool create)
    {
        if (create)
        {
            SetTextInWaiting();
            _gameIdControl = new GameIdControl(_gameId);
            StackPanelPlayer.Children.Insert(0, _gameIdControl);
            
            _playerTurn = true;
            _playerIdTern = SaveRepository.ReadId();
        }
    }

    private void SetTextInWaiting()
    {
        GameTime.Text = "Ожидание";
        GameTime.Foreground = (Brush)new BrushConverter().ConvertFrom("#7074D5");
    }

    public delegate void DeleteMenu(JoinTheRequestControl menu);
    private JoinTheRequestControl? _joinTheRequestControl;
    private void WebSocketOnJoinTheGame(object? sender, JoinTheGame e)
    {
        if (_joinTheRequestControl == null)
        {
            DeleteMenu deleteMenu = menu =>
            {
                _joinTheRequestControl = null;
                StackPanelPlayer.Children.Remove(menu);
            };
            _joinTheRequestControl = new JoinTheRequestControl(_gameId, deleteMenu);
            StackPanelPlayer.Children.Insert(0 , _joinTheRequestControl);
            
            _joinTheRequestControl.AddPlayer(e.Nickname, e.PersonId);
            return;
        }
        _joinTheRequestControl.AddPlayer(e.Nickname, e.PersonId);
    }
    
    private void WebSocketOnUpdateColor(object? sender, UpdateColorPlayer e)
    {
        if (_colorMenu != null)
            _colorMenu.UpdateColor(e.Color);
    }

    private void WebSocketOnRemovePlayer(object? sender, RemovePlayer e)
    {
        if (_anActivePlayers.ContainsKey(e.PlayerId))
        {
            OfflinePlayers.Children.Remove(_anActivePlayers[e.PlayerId]);
            _anActivePlayers.Remove(e.PlayerId);
        }

        if (!_gameState && _players.ContainsKey(e.PlayerId))
        {
            StackPanelPlayer.Children.Remove(_players[e.PlayerId]);
            _players.Remove(e.PlayerId);
        }
    }
    
    private void WebSocketOnGameFinished(object? sender, FinishGame e)
    {
        if (e.Winner == SaveRepository.ReadId())
            MessageBox.Show("Поздравляем \n\nИгра была завершена. Вы победили", "Игра завершена");
        else
        {
            if (_players.ContainsKey(e.Winner))
            {
                string nickname = _players[e.Winner].GetNickname();
                MessageBox.Show($"Игра завершена. \n\nВы проиграли!\nПобедил игрок: {nickname}.", "Игра завершена");
            }
            else
                MessageBox.Show($"Игра завершена. \n\nВы проиграли!", "Игра завершена");
        }
        _mainMenu.OpenMainMenu();
    }

    private Dictionary<string, PlayerTimeMenu> _anActivePlayers = new ();
    private void WebSocketOnIsActivePlayer(object? sender, PlayerIsActive e)
    {
        if (e.State)
        {
            if (_anActivePlayers.ContainsKey(e.PlayerId))
            {
                OfflinePlayers.Children.Remove(_anActivePlayers[e.PlayerId]);
                _anActivePlayers.Remove(e.PlayerId);
            }
        }
        else
        {
            if (!_anActivePlayers.ContainsKey(e.PlayerId))
            {
                PlayerTimeMenu anActivePlayer = new PlayerTimeMenu(e.Nickname, "не в сети", e.Time);
                anActivePlayer.IsOffline();
                _anActivePlayers.Add(e.PlayerId, anActivePlayer);
                OfflinePlayers.Children.Add(anActivePlayer);
            }
        }
    }
    private void WebSocketOnReverseTimeAnActivePlayer(object? sender, ReversTimeAnActivePlayer e)
    {
        if (_anActivePlayers.ContainsKey(e.PlayerId))
        {
            PlayerTimeMenu anActivePlayer = _anActivePlayers[e.PlayerId];
            anActivePlayer.UpdateTime(e.Time);
        }
    }
    
    
    private GameOverMessage GameOver;
    private void WebSocketOnGameOverPlayer(object? sender, GameOverPlayer e)
    {
        if (_players.ContainsKey(e.PersonId))
            _players[e.PersonId].IsGameOver();

        if (e.PersonId == SaveRepository.ReadId())
        {
            ContinueGame continueGame = () => { MainGrid.Children.Remove(GameOver); };
            ExitGame exitGame = () => { _mainMenu.OpenMainMenu(); };
            
            GameOver = new GameOverMessage(exitGame, continueGame);

            Grid.SetRowSpan(GameOver, 3);
            Grid.SetColumnSpan(GameOver, 3);
            
            MainGrid.Children.Add(GameOver);
            Panel.SetZIndex(GameOver, 2);
        }
    }

    private void WebSocketOnReverseTimer(object? sender, ReverseTimer e)
    {
        ReverseTime.Visibility = Visibility.Visible;
        ReverTimeText.Text = e.Time.ToString();

        if (e.Time == 0)
        {
            ReverseTime.Visibility = Visibility.Hidden;
            _gameState = true;
            
            if (_gameIdControl != null) StackPanelPlayer.Children.Remove(_gameIdControl);
        }
    }

    private async Task GetGameData()
    {
        string url = Url.BaseUrl + "Game/playing-field?gameId=" + _gameId;
        
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SaveRepository.ReadToken());

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseContent = await response.Content.ReadAsStringAsync();
                var gameData = JsonSerializer.Deserialize<GameData>(responseContent);

                if (gameData.GameState == GameState.WaitingForPlayers)
                    SetTextInWaiting();

                _playerIdTern = gameData.CurrentPlayer;
                _gameSize = gameData.Board.Count;
                GameName.Text = gameData.GameName;
                GenerateChessBoard(_gameSize);

                SearchYouColor(gameData.Board);
                AddPersons(gameData.Players);
                UpdateBoard(gameData.Board);
                
                MainGrid.Children.Remove(ProcessingGame);
            }
            catch (HttpRequestException e)
            {
                MessageBox.Show($"Ошибка HTTP: {e.Message} \n\n{e.StackTrace}");
            }
            catch (JsonException e)
            {
                MessageBox.Show($"Ошибка при десериализации JSON: {e.Message} \n\n{e.StackTrace}");
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.Message} \n\n{e.StackTrace}");
            }
        }
    }

    private void SearchYouColor(List<List<GameBoard?>> board)
    {
        for (int i = 0; i < board.Count; i++)
        {
            for (int j = 0; j < board[i].Count; j++)
            {
                var piece = board[i][j];

                if (piece != null)
                {
                    if (piece.PersonId == SaveRepository.ReadId())
                    {
                        CreateColorMenu(piece.Color);
                        return;
                    }
                }
            }
        }
    }

    private YouColorMenu? _colorMenu;
    private void CreateColorMenu(string color)
    {
        if (_colorMenu == null)
        {
            _colorMenu = new YouColorMenu(color);
            RightStackPanel.Children.Insert(0, _colorMenu);
        }
    }

    private void WebSocketOnUpdateBoard(object? sender, UpdateBoard e)
    {
        UpdateBoard(e.Board);
    }
    
    private void UpdateBoard(List<List<GameBoard>> board)
    {
        ChessBoardGrid.Children
            .OfType<Image>()
            .ToList()
            .ForEach(img => ChessBoardGrid.Children.Remove(img));

        
        for (int i = 0; i < board.Count; i++)
        {
            for (int j = 0; j < board[i].Count; j++)
            {
                var piece = board[i][j];
                
                int rowCopy = i;
                int colCopy = j;

                if (piece == null)
                    continue;

                string color = piece.Color;
                var type = piece.Type;

                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string fullPath = Path.Combine(basePath, "Image", "Temporary", color, $"{type}.png");

                Image image = new Image
                {
                    Source = new BitmapImage(new Uri(fullPath, UriKind.Absolute)),
                    Margin = new Thickness(_gameSize == 8? 7: 3),
                    Cursor = Cursors.Hand
                };
                
                Panel.SetZIndex(image, 3);

                image.MouseLeftButtonDown += (sender, args) =>
                {
                    if (_gameState && _playerTurn) ClickTheGameBoard(piece.Type, rowCopy, colCopy);
                };
                
                Grid.SetRow(image, i);
                Grid.SetColumn(image, j);
                ChessBoardGrid.Children.Add(image);
            }
        }
    }

    private Border? _selectionHighlight;
    private bool isSelect;
    private AddressTheBoard? StartPoint;
    private void ClickTheGameBoard(PieceType? type, int row, int col)
    {
        if (isSelect && StartPoint?.Row == row && StartPoint?.Col == col)
        {
            StartPoint = null;
            isSelect = false;
            return;
        }
        
        if (!isSelect)
        {
            if (type != null)
            {
                StartPoint = new AddressTheBoard { Row = row, Col = col };
                isSelect = true;
            }
            else
            {
                MessageBox.Show("Здесь нет фигуры");
            }
        }
        else
        {
            if (StartPoint != null)
            {
                _ = SendMoveRequestAsync(StartPoint.Row, StartPoint.Col, row, col);
            }
        }
    }
    
    /// <summary>
    /// Добавляет белый квадрат под выделенную фигуру
    /// </summary>
    private void ShowSelectionHighlight(int row, int col)
    {
        RemoveSelectionHighlight(); // Убираем предыдущее выделение

        _selectionHighlight = new Border
        {
            Background = Brushes.White,
            CornerRadius = new CornerRadius(5),
            Margin = new Thickness(3)
        };

        Grid.SetRow(_selectionHighlight, row);
        Grid.SetColumn(_selectionHighlight, col);
        // Устанавливаем ZIndex ниже фигур
        Panel.SetZIndex(_selectionHighlight, 1);

        // Вставляем выделение перед всеми изображениями
        ChessBoardGrid.Children.Insert(0, _selectionHighlight);
    }

    /// <summary>
    /// Удаляет выделение фигуры
    /// </summary>
    private void RemoveSelectionHighlight()
    {
        if (_selectionHighlight != null)
        {
            ChessBoardGrid.Children.Remove(_selectionHighlight);
            _selectionHighlight = null;
        }

        StartPoint = null;
        isSelect = false;
    }

    private async Task SendMoveRequestAsync(int fromRow, int fromCol, int toRow, int toCol)
    {
        MovePiece movePiece = new MovePiece
        {
            Type = "MovePiece",
            gameId = _gameId,
            token = SaveRepository.ReadToken(),
            FromRow = fromRow,
            FromCol = fromCol,
            ToRow = toRow,
            ToCol = toCol,
        };

        await _webSocketService.SendMessage(movePiece);
        
        isSelect = false;
        StartPoint = null;
    }

    private void WebSocketOnRemainingTimePerson(object? sender, RemainingTimePerson e)
    {
        if (!_players.ContainsKey(e.PersonId))
            return;

        if (_playerIdTern != e.PersonId)
        {
            _players[_playerIdTern].EndTern();
            _playerIdTern = e.PersonId;
        }
        
        _players[_playerIdTern].UpdateTime(e.Time);

        if (_playerIdTern == SaveRepository.ReadId())
        {
            _players[_playerIdTern].IsTern(true);
            _playerTurn = true;
        }
        else
        {
            _players[_playerIdTern].IsTern();
            _playerTurn = false;
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

    private void GenerateChessBoard(int sizeBoard)
    {
        ChessBoardGrid.Children.Clear();
        
        for (int i = 0; i < sizeBoard; i++)
        {
            ChessBoardGrid.RowDefinitions.Add(new RowDefinition());
            ChessBoardGrid.ColumnDefinitions.Add(new ColumnDefinition());
        }
        
        Brush lightColor = (Brush)new BrushConverter().ConvertFrom("#E0E0E0");
        Brush darkColor = (Brush)new BrushConverter().ConvertFrom("#BEBEBE");
        
        for (int row = 0; row < sizeBoard; row++)
        {
            for (int col = 0; col < sizeBoard; col++)
            {
                int rowCopy = row;
                int colCopy = col;
                
                Border cell = new Border
                {
                    Background = (row + col) % 2 == 0 ? lightColor : darkColor,
                    CornerRadius = GetCornerRadius(row, col, sizeBoard),
                    Cursor = Cursors.Hand
                };

                cell.MouseLeftButtonDown += (sender, args) =>
                {
                    if (_gameState && _playerTurn) ClickTheGameBoard(null, rowCopy, colCopy);
                };
                
                Grid.SetRow(cell, row);
                Grid.SetColumn(cell, col);
                ChessBoardGrid.Children.Add(cell);
            }
        }
    }
    
    private CornerRadius GetCornerRadius(int row, int col, int sizeBoard)
    {
        int radius = 5;
        
        if (row == 0 && col == 0)
            return new CornerRadius(radius, 0, 0, 0);
        if (row == sizeBoard - 1 && col == 0)
            return new CornerRadius(0, 0, 0, radius);
        if (row == 0 && col == sizeBoard - 1)
            return new CornerRadius(0, radius, 0, 0);
        if (row == sizeBoard - 1 && col == sizeBoard - 1)
            return new CornerRadius(0, 0, radius, 0);
    
        return new CornerRadius(0);
    }

    private void UpdateGameTime(object? o, DurationGame game)
    {
        _gameState = true;
        
        if (GameTime.Foreground != Brushes.Black)
            GameTime.Foreground = Brushes.Black;
        
        GameTime.Text = game.Time.ToString();
    }

    private void LeaveGame_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var result = MessageBox.Show("Вы уверены что хотите выйти из игры! \nЕсли вы продолжите, то покинете игру и сможете вернуться только в качестве зрителя, так как будете дисквалифицированы. \n\nПосле завершения игры возможна потеря очков.", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Information);
        
        if (result == MessageBoxResult.Yes)
        {
            _ = SendRequestExitGame();
            _mainMenu.OpenMainMenu();
        }
    }

    private async Task SendRequestExitGame()
    {
        HttpClient httpClient = new HttpClient();
        
        var requestUri = Url.BaseUrl + $"Game/leave-game?gameId={_gameId}";

        using var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", SaveRepository.ReadToken());
        
        request.Content = new StringContent(string.Empty);
        await httpClient.SendAsync(request);
    }
}

public class AddressTheBoard
{
    public int Row { get; set; }
    public int Col { get; set; }
}