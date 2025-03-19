using Backend.Enums;
using Backend.Game.GameModes;
using Backend.Game.Potion;
using Backend.Game.Shapes;
using Backend.Services;

namespace Backend.Game;

/// <summary>
/// Игрок
/// </summary>
public class ChessPlayer: IDisposable
{
    public string Id { get; } 
    public string Name { get; }
    public string Color { get; set; }
    
    public bool IsApproved { get; private set; } // Ждет подтверждения
    public List<ChessPiece> Pieces { get; } = new(); // Список фигур игрока
    public PlayerState State { get; set; } = PlayerState.Active; // Статус игрока
    public TimeSpan RemainingTime { get; private set; }
    private Timer? _turnTimer;
    private DateTime? _turnStartTime;
    private IGameMode _gameMode;

    private List<ChessPiece> KillPiece = new ();
    public bool DoubleScoreForNextKill { get; set; } = false;
    
    private List<PotionType> UsedPotion { get; set; } = new();
    public List<PotionType> AvailablePotion { get; set; } = new();

    private int _score;

    public int Score
    {
        get => _score;
        set
        {
            if (DoubleScoreForNextKill)
            {
                _score = value * 2;
                DoubleScoreForNextKill = false;
            }
            else
                _score = value;
            _ = _message.SendMessageUpdateScore(this, _score);
        }
    }

    public void AddUsedPotion(IPotion potion)
    {
        
        if (UsedPotion.Contains(potion.Type))
            UsedPotion.Remove(potion.Type);
    }

    public bool CheckUsedPotion(IPotion potion)
    {
        return !UsedPotion.Contains(potion.Type);
    }
    
    public List<PotionType> GetUsedPotions()
    {
        Console.WriteLine($"A: {AvailablePotion.Count} || U: {UsedPotion.Count}");
        return AvailablePotion.Except(UsedPotion).ToList();
    }
    
    // Событие, которое вызывается, когда время обновляется
    public event Func<ChessPlayer, TimeSpan, Task>? OnTimeUpdate;
    // Событие, закончилось время у игрока
    public event Func<ChessPlayer, Task>? OnTimeIsLost;
    private readonly SendWebSocketMessage _message;
    
    public ChessPlayer(string id, string name, SendWebSocketMessage message, IGameMode mode, List<PotionType> potions)
    {
        Id = id;
        Name = name;
        _message = message;
        _gameMode = mode;
        
        UsedPotion = new List<PotionType>(potions);
        AvailablePotion = new List<PotionType>(potions);

        RemainingTime = mode.GetPlayerTimeDuration();
    }

    public async Task AddKillPiece(ChessPiece piece)
    {
        KillPiece.Add(piece);
        await _message.SendMessageUpdateKillPiece(this, KillPiece);
    }

    public List<PieceType> GetListKillPiece()
    {
        List<PieceType> pieces = KillPiece.Select(p => p.Type).ToList();
        return pieces;
    }
    
    /// <summary>
    /// Одобряет игрока для входа в игру
    /// </summary>
    public void Approve() => IsApproved = true;

    /// <summary>
    /// Начало хода – фиксируем время
    /// </summary>
    public void StartTurn()
    {
        _turnStartTime = DateTime.UtcNow;
        _turnTimer = new Timer(_ => Task.Run(UpdateTime), null, 1000, 1000);
    }

    public TimeSpan GetRemainingTime()
    {
        if (_turnStartTime == null)
            return TimeSpan.Zero;

        return DateTime.UtcNow - _turnStartTime.Value;
    }

    /// <summary>
    /// Завершение хода
    /// </summary>
    public void EndTurn()
    {
        if (_turnStartTime.HasValue)
        {
            _turnTimer?.Dispose();
            _turnTimer = null;
            RemainingTime += _gameMode.GetIncrementTime();
            _ = OnTimeUpdate?.Invoke(this, RemainingTime);
        }
    }
    
    private async Task UpdateTime()
    {
        if (State != PlayerState.Active)
            return;
        
        RemainingTime -= TimeSpan.FromSeconds(1);

        if (RemainingTime <= TimeSpan.Zero)
        {
            RemainingTime = TimeSpan.Zero;
            _turnTimer?.Dispose();
            _turnTimer = null;
            State = PlayerState.OutOfTime;
            await OnTimeIsLost?.Invoke(this);
        }

        await OnTimeUpdate?.Invoke(this, RemainingTime);
    }

    /// <summary>
    /// Проверяет, закончилось ли время у игрока
    /// </summary>
    public bool IsOutOfTime() => RemainingTime <= TimeSpan.Zero;

    private bool _disposed = false;
    public void Dispose()
    {
        if (!_disposed)
        {
            OnTimeUpdate = null;
            OnTimeIsLost = null;

            _turnTimer?.Dispose();
            _turnTimer = null;

            KillPiece.Clear();
            Pieces.Clear();

            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
    
    ~ChessPlayer()
    {
        Console.WriteLine("Вызов финализатора -игрока");
        Dispose();
    }
}