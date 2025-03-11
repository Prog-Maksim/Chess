using Backend.Enums;
using Backend.Game.Shapes;
using Backend.Services;

namespace Backend.Game;

/// <summary>
/// Игрок
/// </summary>
public class ChessPlayer
{
    public string Id { get; } 
    public string Name { get; }
    public string? Color { get; set; }
    
    public bool IsApproved { get; private set; } // Ждет подтверждения
    public List<ChessPiece> Pieces { get; } = new(); // Список фигур игрока
    public PlayerState State { get; set; } = PlayerState.Active; // Статус игрока
    public TimeSpan RemainingTime { get; private set; } = TimeSpan.FromMinutes(25); // Время на ход
    private Timer? _turnTimer;
    private DateTime? _turnStartTime;

    private List<ChessPiece> KillPiece = new ();

    private int _score;

    public int Score
    {
        get => _score;
        set
        {
            _score = value;
            _ = _message.SendMessageUpdateScore(this, _score);
        }
    }
    
    // Событие, которое вызывается, когда время обновляется
    public event Func<ChessPlayer, Task>? OnTimeUpdate;
    private readonly SendWebSocketMessage _message;
    
    public ChessPlayer(string id, string name, SendWebSocketMessage message)
    {
        Id = id;
        Name = name;
        _message = message;
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

    /// <summary>
    /// Завершение хода – уменьшаем оставшееся время
    /// </summary>
    public void EndTurn()
    {
        if (_turnStartTime.HasValue)
        {
            _turnTimer?.Dispose();
            _turnTimer = null;
        }
    }
    
    private async Task UpdateTime()
    {
        if (State != PlayerState.Active)
            return;
            
        if (RemainingTime <= TimeSpan.Zero)
        {
            RemainingTime = TimeSpan.Zero;
            _turnTimer?.Dispose();
            _turnTimer = null;
            State = PlayerState.OutOfTime;
        }
        else
        {
            RemainingTime -= TimeSpan.FromSeconds(1);
        }

        await OnTimeUpdate?.Invoke(this); // Вызываем событие для главного класса
    }

    /// <summary>
    /// Проверяет, закончилось ли время у игрока
    /// </summary>
    public bool IsOutOfTime() => RemainingTime <= TimeSpan.Zero;
}