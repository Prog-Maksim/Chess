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
using Frontend.Controls.Potion;
using Frontend.Controls.Message;
using Frontend.Controls.ResultGame;
using Frontend.Enums;
using Frontend.Models;
using Frontend.Models.Request;
using Frontend.Models.WebSocketMessage;
using Frontend.Script;

namespace Frontend.Windows.Game;

public partial class GameMenu : Page, IDisposable
{
    // состояние игры
    private bool _gameState;
    // состояние игрока, может ли он походить
    private bool _playerTurn;
    // идентификатор карты
    public string GameId;
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
        GameId = gameId;
        _players = new Dictionary<string, PlayerTimeMenu>();
        _mainMenu = mainMenu;
        _webSocketService = webSocket;
        
        SubscribeEvents();
        
        _ = GetGameData();
        WaitingGame(create);
    }
    
    private void SubscribeEvents()
    {
        _webSocketService.OnIsConnected += ClientOnIsConnected;
        _webSocketService.OnConnectRetry += ClientOnConnectRetry;
        _webSocketService.OnFailedConnect += ClientOnFailedConnect;
        
        _webSocketService.OnJoinTheGame += WebSocketOnJoinTheGame;
        _webSocketService.OnDurationGame += UpdateGameTime;
        _webSocketService.OnAddPerson += WebSocketOnAddPerson;
        _webSocketService.OnRemainingTimePerson += WebSocketOnRemainingTimePerson;
        _webSocketService.OnUpdateBoard += WebSocketOnUpdateBoard;
        _webSocketService.OnReverseTimer += WebSocketOnReverseTimer;
        _webSocketService.OnGameOverPlayer += WebSocketOnGameOverPlayer;
        _webSocketService.OnIsActivePlayer += WebSocketOnIsActivePlayer;
        _webSocketService.OnReverseTimeAnActivePlayer += WebSocketOnReverseTimeAnActivePlayer;
        _webSocketService.OnGameFinished += WebSocketOnGameFinished;
        _webSocketService.OnRemovePlayer += WebSocketOnRemovePlayer;
        _webSocketService.OnUpdateColor += WebSocketOnUpdateColor;
        _webSocketService.OnUpdateKillPiece += WebSocketOnUpdateKillPiece;
        _webSocketService.OnUpdateScore += WebSocketOnUpdateScore;
        _webSocketService.OnNewMove += WebSocketOnNewMove;
        _webSocketService.OnUpdateGameState += WebSocketOnUpdateGameState;
        _webSocketService.OnUsePotion += WebSocketOnUsePotion;
        _webSocketService.OnResultTheGame += WebSocketServiceOnResultTheGame;
    }

    private void UnsubscribeEvents()
    {
        _webSocketService.OnIsConnected -= ClientOnIsConnected;
        _webSocketService.OnConnectRetry -= ClientOnConnectRetry;
        _webSocketService.OnFailedConnect -= ClientOnFailedConnect;
        
        _webSocketService.OnJoinTheGame -= WebSocketOnJoinTheGame;
        _webSocketService.OnDurationGame -= UpdateGameTime;
        _webSocketService.OnAddPerson -= WebSocketOnAddPerson;
        _webSocketService.OnRemainingTimePerson -= WebSocketOnRemainingTimePerson;
        _webSocketService.OnUpdateBoard -= WebSocketOnUpdateBoard;
        _webSocketService.OnReverseTimer -= WebSocketOnReverseTimer;
        _webSocketService.OnGameOverPlayer -= WebSocketOnGameOverPlayer;
        _webSocketService.OnIsActivePlayer -= WebSocketOnIsActivePlayer;
        _webSocketService.OnReverseTimeAnActivePlayer -= WebSocketOnReverseTimeAnActivePlayer;
        _webSocketService.OnGameFinished -= WebSocketOnGameFinished;
        _webSocketService.OnRemovePlayer -= WebSocketOnRemovePlayer;
        _webSocketService.OnUpdateColor -= WebSocketOnUpdateColor;
        _webSocketService.OnUpdateKillPiece -= WebSocketOnUpdateKillPiece;
        _webSocketService.OnUpdateScore -= WebSocketOnUpdateScore;
        _webSocketService.OnNewMove -= WebSocketOnNewMove;
        _webSocketService.OnUpdateGameState -= WebSocketOnUpdateGameState;
        _webSocketService.OnUsePotion -= WebSocketOnUsePotion;
        _webSocketService.OnResultTheGame -= WebSocketServiceOnResultTheGame;
    }

    private void ClientOnFailedConnect(object? sender, EventArgs e)
    {
        MainMenu.RetryConnect deleteNotify = () =>
        {
            RemoveWarningAfterDelay(0);
            _ = _webSocketService.ConnectAsync(SaveRepository.LoadTokenFromFile().AccessToken);
        };
        FailedMessageConnect warningRetryConnect = new FailedMessageConnect(deleteNotify);
        warningRetryConnect.Margin = new Thickness(0, 10, 0, 35);
        Notify.Children.Add(warningRetryConnect);
    }

    private void ClientOnConnectRetry(object? sender, EventArgs e)
    {
        RemoveWarningAfterDelay(0);
        
        WarningRetryConnect warningRetryConnect = new WarningRetryConnect();
        warningRetryConnect.Margin = new Thickness(0, 10, 0, 35);
        Notify.Children.Add(warningRetryConnect);
        
        RemoveWarningAfterDelay();
    }
    
    private void ClientOnIsConnected(object? sender, bool e)
    {
        
    }
    
    private async void RemoveWarningAfterDelay(int time = 1500)
    {
        await Task.Delay(time);
        Notify.Children.Clear();
    }
    
    private void WebSocketServiceOnResultTheGame(object? sender, ResultPlayerTheGame e)
    {
        GameTheResult result = new GameTheResult(e.Status, e.League, e.ScoreData.Score, e.ScoreData.AddScoreWine ?? 0, e.ScoreData.PotionScore ?? 0,
            e.ScoreData.ModeScore, e.ScoreData.TotalScore, e.Rating, e.AddPotion, e.UsedPotions, _mainMenu);

        result.Margin = new Thickness(0, 20, 0, 20);
        
        Grid.SetRowSpan(result, 3);
        Grid.SetColumnSpan(result, 3);
            
        MainGrid.Children.Add(result);
        Panel.SetZIndex(result, 2);
    }

    private void WaitingGame(bool create)
    {
        if (create)
        {
            SetTextInWaiting();
            _gameIdControl = new GameIdControl(GameId);
            StackPanelPlayer.Children.Insert(0, _gameIdControl);
            
            _playerTurn = true;
            _playerIdTern = SaveRepository.LoadTokenFromFile().PersonId;
        }
    }

    private void SetTextInWaiting(string message = "Ожидание", string color = "#7074D5")
    {
        GameTime.Text = message;
        GameTime.Foreground = (Brush)new BrushConverter().ConvertFrom(color);
    }
    
    private void WebSocketOnUsePotion(object? sender, UsePotion e)
    {
        UsePotionMenu potionMenu = new UsePotionMenu(e.PotionType, e.UsePersonName, e.PotionName);
        StackPanelPlayer.Children.Add(potionMenu);
        _ = RemoveMenu(potionMenu);
    }

    private async Task RemoveMenu(UsePotionMenu potionMenu)
    {
        await Task.Delay(3000);
        StackPanelPlayer.Children.Remove(potionMenu);
    }
    
    private string _textGame = String.Empty;
    private void WebSocketOnUpdateGameState(object? sender, GameStateMessage e)
    {
        _gameState = false;
        
        if (e.GameState == GameState.Stopped)
        {
            _textGame = GameTime.Text;
            SetTextInWaiting("Пауза");
        }
        else if (e.GameState == GameState.InProgress)
        {
            _gameState = true;
            SetTextInWaiting(_textGame, "Black");
        }
    }
    
    private void WebSocketOnNewMove(object? sender, NewMove e)
    {
        string nickname = _players[e.Move.PlayerId].GetNickname();
        MoveMenu.AddMove(nickname, e.Move.StartRow, e.Move.StartColumn, e.Move.EndRow, e.Move.EndColumn, e.Move.Duration);
    }
    
    private void WebSocketOnUpdateScore(object? sender, UpdateScore e)
    {
        ScoreMenu.UpdateScore(e.Score);
    }

    private void WebSocketOnUpdateKillPiece(object? sender, KillAllPiece e)
    {
        if (e.KillPiece == null)
            return;

        var pieceCounts = e.KillPiece
            .GroupBy(p => p)
            .ToDictionary(g => g.Key, g => g.Count());
        
        foreach (PieceType pieceType in Enum.GetValues(typeof(PieceType)))
        {
            int count = pieceCounts.GetValueOrDefault(pieceType, 0);
            KillPieceControl.UpdateKillPiece(pieceType, count);
        }
    }

    public delegate void DeleteMenu(JoinTheRequestControl menu);
    private JoinTheRequestControl? _joinTheRequestControl;
    private void WebSocketOnJoinTheGame(object? sender, JoinTheGame e)
    {
        CheckMenuJoinTheGame();
        
        if (_joinTheRequestControl != null)
            _joinTheRequestControl.AddPlayer(e.Nickname, e.PersonId);
    }

    private void CheckMenuJoinTheGame()
    {
        if (_joinTheRequestControl == null)
        {
            DeleteMenu deleteMenu = menu =>
            {
                _joinTheRequestControl = null;
                StackPanelPlayer.Children.Remove(menu);
            };
            _joinTheRequestControl = new JoinTheRequestControl(GameId, deleteMenu);
            StackPanelPlayer.Children.Insert(0 , _joinTheRequestControl);
        }
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

        if (e.PlayerId == SaveRepository.LoadTokenFromFile().PersonId)
            _mainMenu.OpenMainMenu();
    }
    
    private void WebSocketOnGameFinished(object? sender, FinishGame e)
    {
        // string time = $"{e.DurationGame.Hours:D2}:{e.DurationGame.Minutes:D2}:{e.DurationGame.Seconds:D2}";
        // MessageBox.Show($"Поздравляем \n\nИгра была завершена. \n\nДлительность игры: {time}", "Игра завершена");
        // _mainMenu.OpenMainMenu();
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

        if (e.PersonId == SaveRepository.LoadTokenFromFile().PersonId)
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
        string url = Url.BaseUrl + "Game/playing-field?gameId=" + GameId;
        
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SaveRepository.LoadTokenFromFile().AccessToken);

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

                if (gameData.WaitingPlayers != null)
                {
                    CheckMenuJoinTheGame();

                    foreach (var e in gameData.WaitingPlayers)
                    {
                        if (_joinTheRequestControl != null)
                            _joinTheRequestControl.AddPlayer(e.Nickname, e.PlayerId);
                    }
                }
                
                GenerateChessBoard(_gameSize);

                SearchYouColor(gameData.Board);
                AddPersons(gameData.Players);
                UpdateBoard(gameData.Board);
                
                CreateMenuPotion(gameData.PotionAvailable);
                
                MainGrid.Children.Remove(ProcessingGame);
            }
            catch (HttpRequestException e)
            {
                MessageBox.Show($"Ошибка HTTP: {e.Message} \n\n{e.StackTrace}");
                _mainMenu.OpenMainMenu();
            }
            catch (JsonException e)
            {
                MessageBox.Show($"Ошибка при десериализации JSON: {e.Message} \n\n{e.StackTrace}");
                _mainMenu.OpenMainMenu();
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.Message} \n\n{e.StackTrace}");
                _mainMenu.OpenMainMenu();
            }
        }
    }

    private PotionMenuControl? PotionMenu;
    private void CreateMenuPotion(PotionAvailable? potion)
    {
        if (PotionMenu == null && potion != null)
        {
            PotionMenu = new PotionMenuControl(this, potion);
            RightStackPanel.Children.Add(PotionMenu);
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
                    if (piece.PersonId == SaveRepository.LoadTokenFromFile().PersonId)
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
        
        
        var enlargedPieces = new Dictionary<string, (int row, int col)>();

        for (int i = 0; i < board.Count - 1; i++)
        {
            for (int j = 0; j < board[i].Count - 1; j++)
            {
                var piece = board[i][j];
                if (piece == null || enlargedPieces.ContainsKey(piece.PieceId)) continue;
                
                if (IsEnlargedPiece(board, piece.PieceId, i, j))
                {
                    enlargedPieces[piece.PieceId] = (i, j);
                }
            }
        }
        
        for (int i = 0; i < board.Count; i++)
        {
            for (int j = 0; j < board[i].Count; j++)
            {
                var piece = board[i][j];
                if (piece == null) continue;

                int rowCopy = i;
                int colCopy = j;
                string color = piece.Color;
                var type = piece.Type;

                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string fullPath = Path.Combine(basePath, "Image", "Temporary", color, $"{type}.png");

                Image image = new Image
                {
                    Source = new BitmapImage(new Uri(fullPath, UriKind.Absolute)),
                    Margin = new Thickness(_gameSize == 8 ? 7 : 3),
                    Cursor = Cursors.Hand
                };

                Panel.SetZIndex(image, 3);

                image.MouseLeftButtonDown += (sender, args) =>
                {
                    if (_gameState && _playerTurn) ClickTheGameBoard(piece.Type, rowCopy, colCopy);
                };
                
                if (enlargedPieces.TryGetValue(piece.PieceId, out var topLeft) && topLeft == (i, j))
                {
                    Grid.SetRow(image, i);
                    Grid.SetColumn(image, j);
                    Grid.SetRowSpan(image, 2);
                    Grid.SetColumnSpan(image, 2);
                }
                else if (!enlargedPieces.ContainsKey(piece.PieceId))
                {
                    Grid.SetRow(image, i);
                    Grid.SetColumn(image, j);
                }
                else
                    continue;

                ChessBoardGrid.Children.Add(image);
            }
        }
    }
    
    private bool IsEnlargedPiece(List<List<GameBoard>> board, string pieceId, int row, int col)
    {
        int rows = board.Count;
        int cols = board[0].Count;
        
        if (row + 1 < rows && col + 1 < cols &&
            board[row + 1][col] != null && board[row][col + 1] != null && board[row + 1][col + 1] != null &&
            board[row + 1][col].PieceId == pieceId &&
            board[row][col + 1].PieceId == pieceId &&
            board[row + 1][col + 1].PieceId == pieceId)
        {
            return true;
        }

        return false;
    }
    
    private Border? _selectionHighlight;
    private bool isSelect;
    public AddressTheBoard? StartPoint;
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

    private async Task SendMoveRequestAsync(int fromRow, int fromCol, int toRow, int toCol)
    {
        MovePiece movePiece = new MovePiece
        {
            Type = "MovePiece",
            gameId = GameId,
            token = SaveRepository.LoadTokenFromFile().AccessToken,
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

        if (_playerIdTern == SaveRepository.LoadTokenFromFile().PersonId)
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
        AddPerson(e.PersonId, e.Nickname, e.Time);
    }

    private void AddPersons(List<GamePlayer> persons)
    {
        foreach (var person in persons)
        {
            AddPerson(person.PlayerId, person.Nickname, person.Time);
        }
    }

    private void AddPerson(string personId, string nickname, TimeSpan time)
    {
        if (!_players.ContainsKey(personId))
        {
            PlayerTimeMenu playerTimeMenu = new PlayerTimeMenu(nickname, time);
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
        var result = MessageBox.Show("Вы уверены что хотите выйти из игры?", "Chess-online", MessageBoxButton.YesNo, MessageBoxImage.Information);
        
        if (result == MessageBoxResult.Yes)
            _ = SendRequestExitGame();
    }

    private async Task SendRequestExitGame()
    {
        HttpClient httpClient = new HttpClient();
        
        var requestUri = Url.BaseUrl + $"Game/leave-game?gameId={GameId}";

        using var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", SaveRepository.LoadTokenFromFile().AccessToken);
        
        request.Content = new StringContent(string.Empty);
        var result =  await httpClient.SendAsync(request);

        if (!result.IsSuccessStatusCode)
        {
            Dispose();
            _mainMenu.OpenMainMenu();
        }
    }
    
    
    private bool _disposed;
    
    public void Dispose()
    {
        if (!_disposed)
        {
            UnsubscribeEvents();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }

    ~GameMenu()
    {
        Dispose();
    }
}

public class AddressTheBoard
{
    public int Row { get; set; }
    public int Col { get; set; }
}