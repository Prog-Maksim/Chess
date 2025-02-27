using System.Net.WebSockets;
using Backend.Game.Shapes;

namespace Backend.Game;

/// <summary>
/// Игрок
/// </summary>
public class ChessPlayer
{
    public string Id { get; } 
    public string Name { get; }
    public string? Color { get; set; }
    public bool IsApproved { get; private set; } = false; // Ждет подтверждения
    public List<ChessPiece> Pieces { get; } = new(); // Список фигур игрока
    
    public WebSocket? Client {get; private set;}
    
    public TimeSpan RemainingTime { get; private set; } = TimeSpan.FromMinutes(25); // Время на ход

    private Timer? _turnTimer;
    private DateTime? _turnStartTime;
    
    // Событие, которое вызывается, когда время обновляется
    public event Func<ChessPlayer, Task>? OnTimeUpdate;
    
    public ChessPlayer(string id, string name)
    {
        Id = id;
        Name = name;
    }
    
    public ChessPlayer(string id, string name, WebSocket client): this(id, name)
    {
        Client = client;
    }

    public void AddWebSocket(WebSocket client)
    {
        Client = client;
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
        if (RemainingTime <= TimeSpan.Zero)
        {
            RemainingTime = TimeSpan.Zero;
            _turnTimer?.Dispose();
            _turnTimer = null;
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