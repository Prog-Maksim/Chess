﻿using Backend.Enums;
using Backend.Game.GameModes;
using Backend.Game.Shapes;
using Backend.Models.Response;
using Backend.Models.Response.WebSocketMessage;
using Backend.Repository.Interfaces;
using Backend.Services;

namespace Backend.Game;

public abstract class BaseChessGame
{
    public string GameId { get; set; }
    public string GameName { get; set; }
    public int BoardSize { get; set; }
    public ChessPiece?[,] Board { get; set; }
    public List<ChessPlayer> Players { get; set; } = new();
    private List<ChessPlayer> WaitingPlayers { get; } = new();

    
    private GameState _state = GameState.WaitingForPlayers;
    public GameState State
    {
        get => _state;
        private set
        {
            _state = value;
            _ = WebSocketMessage.Value.SendMessageUpdateGameState(Players, _state);
        }
    }
    public IGameMode Mode { get; set; }
    
    
    private string OwnerId { get; set; }
    public bool IsGamePrivate { get; set; }
    public int CurrentPlayerIndex; 
    private Timer? _gameTimer;
    private TimeSpan GameDurationSeconds { get; set; } = TimeSpan.Zero;
    public List<Move> Moves { get; set; } = new();
    
    
    protected readonly Lazy<SendWebSocketMessage> WebSocketMessage;
    private readonly IUserDataRepository _userDataRepository;
    private GameService.DeleteGame _deleteGame;

    /// <summary>
    /// Инициализация игры
    /// </summary>
    /// <param name="name"></param>
    /// <param name="boardSize"></param>
    /// <param name="mode"></param>
    /// <param name="player"></param>
    /// <param name="webSocketMessage"></param>
    /// <param name="deleteGame"></param>
    /// <param name="userDataRepository"></param>
    protected BaseChessGame(string name, int boardSize, IGameMode mode, ChessPlayer player, Lazy<SendWebSocketMessage> webSocketMessage, 
        GameService.DeleteGame deleteGame, IUserDataRepository userDataRepository)
    {
        Random rnd = new();
        GameId = $"{rnd.Next(1111, 9999)}-{rnd.Next(1111, 9999)}-{rnd.Next(1111, 9999)}";
        Mode = mode;
        
        GameName = name;
        
        BoardSize = boardSize;
        Board = new ChessPiece?[boardSize, boardSize];
        OwnerId = player.Id;
        
        player.Approve();
        _ = AddPlayer(player);
        
        WebSocketMessage = webSocketMessage;
        _userDataRepository = userDataRepository;
        _deleteGame = deleteGame;
    }

    /// <summary>
    /// Инициализация игры
    /// </summary>
    /// <param name="name"></param>
    /// <param name="boardSize"></param>
    /// <param name="mode"></param>
    /// <param name="player"></param>
    /// <param name="isGamePrivate"></param>
    /// <param name="socketMessage"></param>
    /// <param name="deleteGame"></param>
    /// <param name="userDataRepository"></param>
    protected BaseChessGame(string name, int boardSize, IGameMode mode, ChessPlayer player, bool isGamePrivate, 
        Lazy<SendWebSocketMessage> socketMessage, GameService.DeleteGame deleteGame, IUserDataRepository userDataRepository): 
        this(name, boardSize, mode, player, socketMessage, deleteGame, userDataRepository)
    {
        IsGamePrivate = isGamePrivate;
    }
    
    /// <summary>
    /// Метод добавления игроков
    /// </summary>
    /// <param name="player">Обьект игрока</param>
    /// <returns></returns>
    private async Task<bool> AddPlayer(ChessPlayer player)
    {
        if (Players.Count < RequiredPlayers() && Players.All(p => p.Id != player.Id))
        {
            Players.Add(player);
            player.OnTimeUpdate += HandlePlayerTimeUpdate;
            player.OnTimeIsLost += PlayerOnTimeIsLost;
            await InitializePlayerPieces(player);
            
            if (Players.Count == RequiredPlayers())
                new Thread(async () => await StartGame()).Start();
            
            return true;
        }
        return false;
    }

    private async Task PlayerOnTimeIsLost(ChessPlayer arg)
    {
        await UpdateGameBoard();
        await WebSocketMessage.Value.SendMessagePlayerGameOver(Players, arg.Id);
    }

    /// <summary>
    /// Метод вызывается каждую секунду для каждого игрока активного хода 
    /// </summary>
    /// <param name="player"></param>
    protected abstract Task HandlePlayerTimeUpdate(ChessPlayer player, TimeSpan time);
    
    /// <summary>
    /// Метод оставления заявки на вступления в игру
    /// </summary>
    /// <param name="player"></param>
    public async Task RequestJoin(ChessPlayer player)
    {
        var user = Players.FirstOrDefault(p => p.Id == player.Id);

        if (user != null && user.IsApproved)
        {
            await WebSocketMessage.Value.SendMessageResultJoinTheGame(player, GameId, true);
            await WebSocketMessage.Value.SendMessageAddNewPlayer(Players, player);
            return;
        }
        if (!IsGamePrivate)
        {
            player.Approve();
            await AddPlayer(player);

            await WebSocketMessage.Value.SendMessageResultJoinTheGame(player, GameId, true);
            await WebSocketMessage.Value.SendMessageAddNewPlayer(Players, player);
            
            return;
        }
        
        var owner = Players.FirstOrDefault(p => p.Id == OwnerId);
        await WebSocketMessage.Value.SendMessageJoinTheGame(player, owner);
        
        WaitingPlayers.Add(player);
    }
    
    /// <summary>
    /// Метод разрешающий пользователю вступить в игру
    /// </summary>
    /// <param name="ownerId"></param>
    /// <param name="playerId"></param>
    /// <returns></returns>
    public async Task<bool> ApprovePlayer(string ownerId, string playerId)
    {
        if (ownerId != OwnerId) 
            return false;
        var player = WaitingPlayers.Find(p => p.Id == playerId);
        if (player == null) 
            return false;
        
        await WebSocketMessage.Value.SendMessageResultJoinTheGame(player, GameId, true);
        await WebSocketMessage.Value.SendMessageAddNewPlayer(Players, player);

        player.Approve();
        _ = Task.Run(() => AddPlayer(player));
        WaitingPlayers.Remove(player);
        
        return true;
    }
    
    /// <summary>
    /// Метод запрещающий пользователю вступить в игру
    /// </summary>
    /// <param name="ownerId">ID владельца игры</param>
    /// <param name="playerId">ID игрока, который должен быть удален</param>
    /// <returns></returns>
    public async Task<bool> RejectPlayer(string ownerId, string playerId)
    {
        if (ownerId != OwnerId) return false; // Только владелец может отклонить игроков
        var player = WaitingPlayers.Find(p => p.Id == playerId);
        if (player == null) return false; // Игрок не найден в списке ожидания

        // Удаляем игрока из списка ожидания
        WaitingPlayers.Remove(player);
        
        var owner = Players.FirstOrDefault(p => p.Id == OwnerId);
        await WebSocketMessage.Value.SendMessageResultJoinTheGame(player, GameId, false);
        
        return true;
    }

    /// <summary>
    /// Запуск игры и обратного отсчета
    /// </summary>
    private async Task StartGame()
    {
        State = GameState.Countdown;

        for (int i = 5; i > 0; i--)
        {
            await WebSocketMessage.Value.SendMessageReverseTimer(Players, i);
            await Task.Delay(1000);
        }
            
        State = GameState.InProgress;
        StartTimer();
            
        Players[CurrentPlayerIndex].StartTurn();
    }

    protected void NextTurn()
    {
        if (State != GameState.InProgress)
            return;
        
        Players[CurrentPlayerIndex].EndTurn();
        
        if (CurrentPlayerIndex + 1 < Players.Count) CurrentPlayerIndex++;
        else CurrentPlayerIndex = 0;
        
        if (Players[CurrentPlayerIndex].State == PlayerState.Active)
            Players[CurrentPlayerIndex].StartTurn();
        else
            NextTurn();
    }
    
    /// <summary>
    /// Метод для инициализации фишек у игрока, а также их расставление
    /// </summary>
    /// <param name="player">Игрок</param>
    protected abstract Task InitializePlayerPieces(ChessPlayer player);

    /// <summary>
    /// Перерисовывает игровое поле
    /// </summary>
    /// <returns></returns>
    protected abstract Task UpdateGameBoard();
    
    /// <summary>
    /// Добавление фигуры на карту
    /// </summary>
    /// <param name="piece"></param>
    /// <param name="col"></param>
    /// <param name="row"></param>
    protected void AddPieceToBoard(ChessPiece piece, int col, int row)
    {
        Board[col, row] = piece;
    }
    
    /// <summary>
    /// Максимально кол-во игроков
    /// </summary>
    /// <returns></returns>
    protected abstract int RequiredPlayers();
    
    /// <summary>
    /// Счетчик отчета время игры
    /// </summary>
    private void StartTimer()
    {
        _gameTimer = new Timer(_ => Task.Run(UpdateGameTime), null, 1000, 1000);
    }
    
    /// <summary>
    /// Метод обновления времени игры
    /// </summary>
    private async Task UpdateGameTime()
    {
        if (State == GameState.InProgress)
        {
            GameDurationSeconds += TimeSpan.FromSeconds(1);
            await WebSocketMessage.Value.SendMessageTimerGame(Players, GameDurationSeconds);

            await CheckThePlayers();

            if (Players[CurrentPlayerIndex].IsOutOfTime())
                NextTurn();
        }
    }

    /// <summary>
    /// Рассылает сообщения пользователям при обновлении игрового поля
    /// </summary>
    protected virtual async Task SendMessageUpdateBoard()
    {
        GameBoard?[,] gameBoards = new GameBoard?[Board.GetLength(0), Board.GetLength(1)];

        for (int i = 0; i < Board.GetLength(0); i++)
        {
            for (int j = 0; j < Board.GetLength(1); j++)
            {
                if (Board[i, j] == null)
                    gameBoards[i, j] = null;
                else
                {
                    var dataChessPiece = Board[i, j];
                    var color = Players.Find(p => p.Id == dataChessPiece.OwnerId).Color;
                    
                    GameBoard gameBoard = new GameBoard
                    {
                        Color = color,
                        Type = dataChessPiece.Type,
                        PieceId = dataChessPiece.ChessPieceId,
                        PersonId = dataChessPiece.OwnerId
                    };

                    gameBoards[i, j] = gameBoard;
                }
            }
        }
        
        await WebSocketMessage.Value.SendMessageUpdateBoard(Players, gameBoards);
    }
    
    /// <summary>
    /// Остановка таймера
    /// </summary>
    private void StopTimer()
    {
        _gameTimer?.Dispose();
        _gameTimer = null;
    }

    protected async Task AddNewMoveAsync(string playerId, int startRow, int startCol, int endRow, int endCol, TimeSpan timeMove)
    {
        Move move = new Move
        {
            MoveId = Guid.NewGuid().ToString(),
            PlayerId = playerId,
            DataMove = DateTime.Now,
            StartRow = startRow,
            StartColumn = startCol,
            EndRow = endRow,
            EndColumn = endCol,
            Duration = timeMove
        };
        Moves.Add(move);
        await WebSocketMessage.Value.SendMessageNewMoving(Players, move);
    }
    protected abstract Task<bool> Moving(string personId, int oldRow, int oldCol, int newRow, int newCol);

    public async Task<bool> MakeAMove(string personId, int oldRow, int oldCol, int newRow, int newCol)
    {
        if (State == GameState.InProgress)
            return await Moving(personId, oldRow, oldCol, newRow, newCol);

        return false;
    }

    /// <summary>
    /// Объявление победы
    /// </summary>
    protected virtual async Task DeclarationVictory(ChessPlayer player)
    {
        State = GameState.Finished;
        player.Score *= 2;
        
        foreach (var user in Players)
        {
            try
            {
                if (user.State == PlayerState.Disconnected)
                {
                    user.Score = 0;
                    user.Score = -50;
                }

                FinishGame finish = new FinishGame
                {
                    MessageType = "Finished",
                    Message = "Игра завершена",
                    StatusCode = 200,
                    Success = true,
                    WinnerId = player.Id,
                    IsWinner = user.Id == player.Id,
                    Score = player.Score
                };
                await WebSocketMessage.Value.SendMessageFinishGame(user, finish);
                await _userDataRepository.UpdateScore(user.Id, user.Score);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        
        _deleteGame(GameId);
    }

    /// <summary>
    /// Проверка активных игроков
    /// </summary>
    private async Task CheckThePlayers()
    {
        var players = Players.Where(p => p.State == PlayerState.Active).ToList();
        
        if (players.Count == 1)
        {
            if (State == GameState.InProgress)
            {
                State = GameState.Finished;
                
                foreach (var pl in Players)
                    pl.EndTurn();
                
                StoppedAllPlayers();
                await DeclarationVictory(players.First());
            }
            else
                _deleteGame(GameId);
        }
    }
    
    private readonly Dictionary<string, CancellationTokenSource> _playerTimers = new();

    /// <summary>
    /// Отмечаем пользователя как неактивный
    /// </summary>
    /// <param name="playerId"></param>
    public void InActivePlayer(string playerId)
    {
        if (_playerTimers.ContainsKey(playerId))
            return;

        var player = Players.FirstOrDefault(p => p.Id == playerId);
        
        if (State == GameState.Finished)
        {
            player.State = PlayerState.Disconnected;
            return;
        }
        
        if (State == GameState.WaitingForPlayers)
        {
            Players.Remove(player);
            _ = WebSocketMessage.Value.SendMessageRemovePlayer(Players, playerId);
            return;
        }
        
        player.State = PlayerState.InActive;
        State = GameState.Stopped;
        
        var cts = new CancellationTokenSource();
        _playerTimers[playerId] = cts;

        PlayerIsActive playerIsActive = new PlayerIsActive
        {
            MessageType = "PlayerIsActive",
            Message = "Игрок отключился",
            StatusCode = 200,
            Success = true,
            PlayerId = playerId,
            Nickname = player.Name,
            State = false,
            Time = TimeSpan.FromMinutes(1)
        };
        _ = WebSocketMessage.Value.SendMessageStateActivePlayer(Players, playerIsActive);
        

        StoppedAllPlayers();
        Task.Run(async () => PlayerReverseTimerAsync(playerId, cts), cts.Token);
    }

    /// <summary>
    /// Останавливаем всех игроков
    /// </summary>
    private void StoppedAllPlayers()
    {
        foreach (var player in Players)
        {
            if (player.State == PlayerState.Active)
                player.State = PlayerState.Stopped;
        }
    }

    /// <summary>
    /// Возобновляем остановленных игрокв
    /// </summary>
    private void ResumeAllPlayers()
    {
        foreach (var player in Players)
        {
            if (player.State == PlayerState.Stopped)
                player.State = PlayerState.Active;
        }
    }
    
    /// <summary>
    /// Таймер отсчета времени до бана игрока
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="cts"></param>
    private async Task PlayerReverseTimerAsync(string playerId, CancellationTokenSource cts)
    {
        const int timeoutSeconds = 60;
        int remainingTime = timeoutSeconds;
            
        try
        {
            while (remainingTime > 0)
            {
                await Task.Delay(1000, cts.Token);

                remainingTime--;

                var player = Players.FirstOrDefault(p => p.Id == playerId);
                if (player == null || player.State != PlayerState.InActive)
                {
                    return;
                }
                    
                Console.WriteLine($"Игрок {playerId} отключится через {remainingTime} сек.");
                ReversTimeAnActivePlayer reversTime = new ReversTimeAnActivePlayer()
                {
                    MessageType = "InActiveTime",
                    Message = $"У игрока осталось времени: {TimeSpan.FromSeconds(remainingTime)}",
                    StatusCode = 200,
                    Success = true,
                    PlayerId = playerId,
                    Time = TimeSpan.FromSeconds(remainingTime)
                };
                await WebSocketMessage.Value.SendMessageStateReverseTimeInActivePlayer(Players, reversTime);
                Console.WriteLine("Сообщение отправлено");
            }
                
            var disconnectedPlayer = Players.FirstOrDefault(p => p.Id == playerId);
            if (disconnectedPlayer != null && disconnectedPlayer.State == PlayerState.InActive)
            {
                await DeletePieceInAnActivePlayer(playerId);
                await WebSocketMessage.Value.SendMessageRemovePlayer(Players, playerId);
                    
                disconnectedPlayer.State = PlayerState.Disconnected;

                if (Players.Where(p => p.State == PlayerState.InActive).Count() == 0)
                {
                    State = GameState.InProgress;
                    ResumeAllPlayers();
                    await WebSocketMessage.Value.SendMessageRemovePlayer(Players, playerId);
                }
            }
        }
        catch (TaskCanceledException) { }
        finally
        {
            _playerTimers.Remove(playerId);
        }
    }

    private async Task DeletePieceInAnActivePlayer(string playerId)
    {
        for (int i = 0; i < Board.GetLength(0); i++)
        {
            for (int j = 0; j < Board.GetLength(1); j++)
            {
                if (Board[i, j] != null && Board[i, j].OwnerId == playerId)
                {
                    Board[i, j] = null;
                }
            }
        }

        await SendMessageUpdateBoard();
    }

    /// <summary>
    /// Помечаем пользователя снова активным
    /// </summary>
    /// <param name="playerId"></param>
    public void SetPlayerActive(string playerId)
    {
        var player = Players.FirstOrDefault(p => p.Id == playerId);

        if (player != null && player.State == PlayerState.InActive)
        {
            player.State = PlayerState.Active;
            if (_playerTimers.TryGetValue(playerId, out var cts))
            {
                PlayerIsActive playerIsActive = new PlayerIsActive
                {
                    MessageType = "PlayerIsActive",
                    Message = "Игрок подключился",
                    StatusCode = 200,
                    Success = true,
                    PlayerId = playerId,
                    Nickname = player.Name,
                    State = true,
                    Time = TimeSpan.FromSeconds(1)
                };
                _ = WebSocketMessage.Value.SendMessageStateActivePlayer(Players, playerIsActive);
                
                cts.Cancel();
                _playerTimers.Remove(playerId);
            }

            if (Players.Where(p => p.State == PlayerState.InActive).Count() == 0)
            {
                State = GameState.InProgress;
                ResumeAllPlayers();
            }
            
            Console.WriteLine($"Игрок {playerId} снова активен.");
        }
    }

    /// <summary>
    /// Выход игрока из игры
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    public virtual bool ExitTheGame(string playerId)
    {
        var player = Players.FirstOrDefault(p => p.Id == playerId);
        
        if (player == null)
            return false;
        
        _ = WebSocketMessage.Value.SendMessageRemovePlayer(Players, playerId);
        
        if (State == GameState.WaitingForPlayers)
        {
            Players.Remove(player);
            UpdateGameBoard();
            
            return true;
        }

        if (player.State != PlayerState.Active)
            return true;

        player.State = PlayerState.Disconnected;
        return true;
    }
    
    ~BaseChessGame()
    {
        StopTimer();
        _gameTimer?.Dispose();
        _gameTimer = null;

        foreach (var player in Players)
            player.Dispose();
        
        Players.Clear();
        Players = null;
    }
}