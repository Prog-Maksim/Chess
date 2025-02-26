﻿using Backend.Enums;
using Backend.Game.Shapes;
using Backend.Models.Response;
using Backend.Services;

namespace Backend.Game;

public abstract class BaseChessGame
{
    public string GameId { get; set; } = Guid.NewGuid().ToString();
    public string GameName { get; set; }
    public int BoardSize { get; set; }
    public ChessPiece?[,] Board { get; set; }
    public List<ChessPlayer> Players { get; set; } = new();
    public List<ChessPlayer> WaitingPlayers { get; } = new();
    public GameState State { get; set; } = GameState.WaitingForPlayers;
    public string OwnerId { get; set; }
    public bool IsGamePrivate { get; set; } = false;
    public int CurrentPlayerIndex = 0; 
    private Timer? _gameTimer;
    public TimeSpan GameDurationSeconds { get; private set; } = TimeSpan.Zero;
    

    /// <summary>
    /// Инициализация игры
    /// </summary>
    /// <param name="boardSize">Размер поля</param>
    /// <param name="ownerId">Главный пользователь</param>
    protected BaseChessGame(int boardSize, string ownerId)
    {
        BoardSize = boardSize;
        Board = new ChessPiece?[boardSize, boardSize];
        OwnerId = ownerId;
    }
    
    /// <summary>
    /// Инициализация игры
    /// </summary>
    /// <param name="boardSize">Размер поля</param>
    /// <param name="ownerId">Главный пользователь</param>
    /// <param name="isGamePrivate">Приватная ли игра</param>
    protected BaseChessGame(int boardSize, string ownerId, bool isGamePrivate): this(boardSize, ownerId)
    {
        IsGamePrivate = isGamePrivate;
    }

    
    /// <summary>
    /// Метод добавления игроков
    /// </summary>
    /// <param name="player">Обьект игрока</param>
    /// <returns></returns>
    protected virtual async Task<bool> AddPlayer(ChessPlayer player)
    {
        if (Players.Count < RequiredPlayers())
        {
            Players.Add(player);
            player.OnTimeUpdate += HandlePlayerTimeUpdate;
            await InitializePlayerPieces(player);
            
            if (Players.Count == RequiredPlayers())
                await StartGame();
            
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// Метод вызывается каждую секунду для каждого игрока активного хода 
    /// </summary>
    /// <param name="player"></param>
    protected abstract Task HandlePlayerTimeUpdate(ChessPlayer player);
    
    /// <summary>
    /// Метод оставления заявки на вступления в игру
    /// </summary>
    /// <param name="player"></param>
    public virtual async Task RequestJoin(ChessPlayer player)
    {
        if (player.Id == OwnerId || !IsGamePrivate)
        {
            player.Approve();
            await AddPlayer(player);
            return;
        }
        
        var owner = Players.FirstOrDefault(p => p.Id == OwnerId);
        await SendWebSocketMessage.SendMessageJoinTheGame(player, owner);
        
        WaitingPlayers.Add(player);
    }
    
    // TODO: оповестить игрока о том что ему разрешили вступить в игру
    /// <summary>
    /// Метод разрешаюший пользователю вступить в игру
    /// </summary>
    /// <param name="ownerId"></param>
    /// <param name="playerId"></param>
    /// <returns></returns>
    public virtual async Task<bool> ApprovePlayer(string ownerId, string playerId)
    {
        if (ownerId != OwnerId) 
            return false;
        var player = WaitingPlayers.Find(p => p.Id == playerId);
        if (player == null) 
            return false;
        
        await SendWebSocketMessage.SendMessageResultJoinTheGame(player, true);
        await SendWebSocketMessage.SendMessageAddNewPlayer(Players, player);

        player.Approve();
        _ = Task.Run(() => AddPlayer(player));
        WaitingPlayers.Remove(player);
        
        return true;
    }
    
    // TODO: оповестить игрока о том что ему запретили в игре
    /// <summary>
    /// Метод запрещающий пользователю вступить в игру
    /// </summary>
    /// <param name="ownerId">ID владельца игры</param>
    /// <param name="playerId">ID игрока, который должен быть удален</param>
    /// <returns></returns>
    public virtual async Task<bool> RejectPlayer(string ownerId, string playerId)
    {
        if (ownerId != OwnerId) return false; // Только владелец может отклонить игроков
        var player = WaitingPlayers.Find(p => p.Id == playerId);
        if (player == null) return false; // Игрок не найден в списке ожидания

        // Удаляем игрока из списка ожидания
        WaitingPlayers.Remove(player);
        
        var owner = Players.FirstOrDefault(p => p.Id == OwnerId);
        await SendWebSocketMessage.SendMessageResultJoinTheGame(player, false);
        
        return true;
    }

    /// <summary>
    /// Запуск игры и обратного отсчета
    /// </summary>
    public async Task StartGame()
    {
        State = GameState.Countdown;

        for (int i = 5; i > 0; i--)
        {
            await SendWebSocketMessage.SendMessageReverseTimer(Players, i);
            await Task.Delay(1000);
        }
            
        State = GameState.InProgress;
        StartTimer();
            
        Players[CurrentPlayerIndex].StartTurn();
    }

    
    /// <summary>
    /// Метод для инициализации фишек у игрока, а также их расставление
    /// </summary>
    /// <param name="player">Игрок</param>
    protected abstract Task InitializePlayerPieces(ChessPlayer player);
    
    /// <summary>
    /// Добавление фигуры на карту
    /// </summary>
    /// <param name="piece"></param>
    /// <param name="col"></param>
    /// <param name="row"></param>
    public virtual void AddPieceToBoard(ChessPiece piece, int col, int row)
    {
        Board[col, row] = piece;
    }

    /// <summary>
    /// Обновление позиции фигуры на карте
    /// </summary>
    /// <param name="piece"></param>
    /// <param name="oldCol"></param>
    /// <param name="oldRow"></param>
    /// <param name="newCol"></param>
    /// <param name="newRow"></param>
    public virtual void UpdatePieceToBoard(ChessPiece piece, int oldCol, int oldRow, int newCol, int newRow)
    {
        Board[newCol, newRow] = piece;
        Board[oldCol, oldRow] = null;
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
            await SendWebSocketMessage.SendMessageTimerGame(Players, GameDurationSeconds);

            // Если время игры превышено, завершаем
            if (GameDurationSeconds >= MaxGameTimeInSeconds())
            {
                State = GameState.Finished;
                StopTimer();
            }
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
        
        await SendWebSocketMessage.SendMessageUpdateBoard(Players, gameBoards);
    }
    
    /// <summary>
    /// Остановка таймера
    /// </summary>
    public void StopTimer()
    {
        _gameTimer?.Dispose();
        _gameTimer = null;
    }

    /// <summary>
    /// Максимальное время длительности игры
    /// </summary>
    /// <returns></returns>
    protected virtual TimeSpan MaxGameTimeInSeconds() => TimeSpan.FromHours(1);

    ~BaseChessGame()
    {
        StopTimer();

        foreach (var player in Players)
            player.OnTimeUpdate -= HandlePlayerTimeUpdate;
    }
}