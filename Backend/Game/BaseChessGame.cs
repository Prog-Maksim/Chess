using Amazon.S3.Model;
using Backend.Enums;

namespace Backend.Game;

public abstract class BaseChessGame
{
    public string GameId { get; set; } = Guid.NewGuid().ToString();
    public int BoardSize { get; set; }
    public ChessPiece?[,] Board { get; set; }
    public List<ChessPlayer> Players { get; set; } = new();
    public List<ChessPlayer> WaitingPlayers { get; } = new();
    public GameState State { get; set; } = GameState.WaitingForPlayers;
    public string OwnerId { get; set; }
    public bool IsGamePrivate { get; set; } = false;
    
    
    protected int _currentPlayerIndex = 0; 
    

    private Timer? _gameTimer;
    public int GameDurationSeconds { get; private set; } = 0;
    
    // События
    public event Action<string, ChessPlayer>? OnPlayerRequestJoin; // событие присоединения игроков

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
    protected virtual bool AddPlayer(ChessPlayer player)
    {
        if (Players.Count < RequiredPlayers())
        {
            Players.Add(player);
            player.OnTimeUpdate += HandlePlayerTimeUpdate;
            InitializePlayerPieces(player);
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// Метод вызывается каждую секунду для каждого игрока активного хода 
    /// </summary>
    /// <param name="player"></param>
    protected abstract void HandlePlayerTimeUpdate(ChessPlayer player);
    
    /// <summary>
    /// Метод оставления заявки на вступления в игру
    /// </summary>
    /// <param name="player"></param>
    public virtual void RequestJoin(ChessPlayer player)
    {
        if (player.Id == OwnerId || !IsGamePrivate)
        {
            player.Approve();
            AddPlayer(player);
            return;
        }
        
        WaitingPlayers.Add(player);
        OnPlayerRequestJoin?.Invoke(OwnerId, player); // Уведомляем владельца
    }
    
    // TODO: оповестить игрока о том что ему разрешили вступить в игру
    /// <summary>
    /// Метод разрешаюший пользователю вступить в игру
    /// </summary>
    /// <param name="ownerId"></param>
    /// <param name="playerId"></param>
    /// <returns></returns>
    public virtual bool ApprovePlayer(string ownerId, string playerId)
    {
        if (ownerId != OwnerId) return false; // Только владелец может одобрять
        var player = WaitingPlayers.Find(p => p.Id == playerId);
        if (player == null) return false;

        player.Approve();
        AddPlayer(player);
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
    public virtual bool RejectPlayer(string ownerId, string playerId)
    {
        if (ownerId != OwnerId) return false; // Только владелец может отклонить игроков
        var player = WaitingPlayers.Find(p => p.Id == playerId);
        if (player == null) return false; // Игрок не найден в списке ожидания

        // Удаляем игрока из списка ожидания
        WaitingPlayers.Remove(player);
        return true;
    }

    /// <summary>
    /// Начало игры
    /// </summary>
    public void StartGame()
    {
        if (Players.Count == RequiredPlayers())
        {
            State = GameState.InProgress;
            InitializeBoard();
            StartTimer(); // Запускаем таймер
            
            // Указываем что данный игрок начинает игру
            // Players[_currentPlayerIndex].StartTurn();
        }
    }

    /// <summary>
    /// Метод для инициализации фишек у игрока, а также их расставление
    /// </summary>
    /// <param name="player"></param>
    protected abstract void InitializePlayerPieces(ChessPlayer player);
    
    /// <summary>
    /// Максимально кол-во игроков
    /// </summary>
    /// <returns></returns>
    protected abstract int RequiredPlayers();
    
    /// <summary>
    /// Метод инициализации игрового поля
    /// </summary>
    protected abstract void InitializeBoard();
    
    /// <summary>
    /// Счетчик отчета время игры
    /// </summary>
    private void StartTimer()
    {
        _gameTimer = new Timer(UpdateGameTime, null, 1000, 1000);
    }
    
    /// <summary>
    /// Время обновления времени игры
    /// </summary>
    /// <param name="state"></param>
    private void UpdateGameTime(object? state)
    {
        if (State == GameState.InProgress)
        {
            GameDurationSeconds++;

            // Если время игры превышено, завершаем
            if (GameDurationSeconds >= MaxGameTimeInSeconds())
            {
                State = GameState.Finished;
                StopTimer();
            }
        }
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
    protected virtual int MaxGameTimeInSeconds() => 3600;
}