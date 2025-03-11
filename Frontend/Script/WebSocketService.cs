using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Windows;
using Frontend.Models.Request;
using Frontend.Models.WebSocketMessage;

namespace Frontend.Script;

public class WebSocketService
{
    private static WebSocketService _instance;
    private static readonly object _lock = new ();
    private static ClientWebSocket? client;
    private readonly int _maxRetries = 3;
    private int _retryCount;
    private readonly TimeSpan _retryDelay = TimeSpan.FromSeconds(3);
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    /// <summary>
    /// Событие Активен ли WebSocket
    /// </summary>
    public event EventHandler<bool>? OnIsConnected; 
    /// <summary>
    /// Событие при не успешном подключении к WebWocket
    /// </summary>
    public event EventHandler? OnConnectRetry; 
    /// <summary>
    /// Не удалось подключиться к WebWocket
    /// </summary>
    public event EventHandler? OnFailedConnect; 

    private WebSocketService()
    {
        
    }
    
    public static WebSocketService Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new WebSocketService();
                }
                return _instance;
            }
        }
    }
    
    public async Task ConnectAsync(string token)
    {
        if (client?.State == WebSocketState.Open || client?.State == WebSocketState.Connecting)
            return;

        client = new ClientWebSocket();
        client.Options.SetRequestHeader("Authorization", $"Bearer {token}");

        while (_retryCount < _maxRetries)
        {
            try
            {
                Console.WriteLine("Connecting to WebSocket...");
                await client.ConnectAsync(new Uri(Url.BaseUrlWs), _cancellationTokenSource.Token);

                Console.WriteLine("WebSocket connected!");
                OnIsConnected?.Invoke(this, true);
                _retryCount = 0;

                await ReceiveMessages();
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WebSocket connection error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                OnIsConnected?.Invoke(this, false);
                _retryCount++;

                if (_retryCount >= _maxRetries)
                {
                    Console.WriteLine("Max retry attempts reached. Connection failed.");
                    break;
                }
                
                OnConnectRetry?.Invoke(this, EventArgs.Empty);
                Console.WriteLine($"Retrying connection in {_retryDelay.TotalSeconds} seconds...");
                await Task.Delay(_retryDelay);
            }
        }

        if (_retryCount >= _maxRetries)
        {
            _retryCount = 0;
            OnFailedConnect?.Invoke(this, EventArgs.Empty);
        }
    }
    
    public async Task DisconnectAsync()
    {
        if (client == null || client.State == WebSocketState.Closed || client.State == WebSocketState.Aborted)
            return;

        try
        {
            Console.WriteLine("Closing WebSocket connection...");
            await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while closing WebSocket: {ex.Message}");
        }
        finally
        {
            client.Dispose();
            client = null;
        }
        
        OnIsConnected?.Invoke(this, false);
    }
    
    public bool IsConnected()
    {
        return client != null && client.State == WebSocketState.Open;
    }

    public async Task SendMessage(MovePiece movePiece)
    {
        var message = JsonSerializer.Serialize(movePiece);
        var buffer = Encoding.UTF8.GetBytes(message);
        
        if (client != null)
            await client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, 
                CancellationToken.None);
    }
    
    private async Task ReceiveMessages()
    {
        var buffer = new byte[8192];
        using var ms = new MemoryStream();

        while (client.State == WebSocketState.Open)
        {
            ms.SetLength(0);
            WebSocketReceiveResult result;
            do
            {
                result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
    
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Сервер закрыл соединение", CancellationToken.None);
                    Console.WriteLine("Сервер закрыл соединение.");
                    return;
                }

                ms.Write(buffer, 0, result.Count);

            } while (!result.EndOfMessage);
        
            string message = Encoding.UTF8.GetString(ms.ToArray());
            Console.WriteLine(message);
            await ParseMessages(message);
        }
    }
    
    /// <summary>
    /// Заявка на вступление в игру
    /// </summary>
    public event EventHandler<JoinTheGame>? OnJoinTheGame;
    /// <summary>
    /// Ответ на заявку
    /// </summary>
    public event EventHandler<ResultJoinTheGame>? OnResultJoinTheGame;
    /// <summary>
    /// Обратный отсчет до начала игры
    /// </summary>
    public event EventHandler<ReverseTimer>? OnReverseTimer;
    /// <summary>
    /// Событие длительности игры
    /// </summary>
    public event EventHandler<DurationGame>? OnDurationGame;
    /// <summary>
    /// Оставшееся время игры у пользователя
    /// </summary>
    public event EventHandler<RemainingTimePerson>? OnRemainingTimePerson;
    /// <summary>
    /// Событие о добавление нового пользователя
    /// </summary>
    public event EventHandler<AddPerson>? OnAddPerson;
    /// <summary>
    /// Событие обновление игрового поля
    /// </summary>
    public event EventHandler<UpdateBoard>? OnUpdateBoard;
    /// <summary>
    /// Событие завершения игры
    /// </summary>
    public event EventHandler<FinishGame>? OnGameFinished;
    /// <summary>
    /// Событие проигрыша игрока
    /// </summary>
    public event EventHandler<GameOverPlayer>? OnGameOverPlayer;
    /// <summary>
    /// Событие статуса пользователя (активен не активен)
    /// </summary>
    public event EventHandler<PlayerIsActive>? OnIsActivePlayer;
    /// <summary>
    /// Время, которое осталось у игрока на перезаход в игру
    /// </summary>
    public event EventHandler<ReversTimeAnActivePlayer>? OnReverseTimeAnActivePlayer;
    /// <summary>
    /// Игрок был удален из игры или выбыд
    /// </summary>
    public event EventHandler<RemovePlayer>? OnRemovePlayer;
    /// <summary>
    /// У игрока изменился цвет
    /// </summary>
    public event EventHandler<UpdateColorPlayer>? OnUpdateColor;
    /// <summary>
    /// Событие обновления кол-ва убитых фигур
    /// </summary>
    public event EventHandler<KillAllPiece>? OnUpdateKillPiece;
    /// <summary>
    /// События обновления заработанных очков
    /// </summary>
    public event EventHandler<UpdateScore>? OnUpdateScore;

    private async Task ParseMessages(string message)
    {
        if (message.Contains("JoinResult"))
        {
            var result = JsonSerializer.Deserialize<ResultJoinTheGame>(message);
            
            if (result != null)
                OnResultJoinTheGame?.Invoke(this, result);
        }
        else if (message.Contains("Join"))
        {
            Console.WriteLine(message);
            var result = JsonSerializer.Deserialize<JoinTheGame>(message);
            
            if (result != null)
                OnJoinTheGame?.Invoke(this, result);
        }
        else if (message.Contains("ReverseTimer"))
        {
            var result = JsonSerializer.Deserialize<ReverseTimer>(message);
            
            if (result != null)
            {
                OnReverseTimer?.Invoke(this, result);

                try
                {
                    if (result.Time == 1)
                        await Task.Run(async () =>
                        {
                            await Task.Delay(1000);
                            ReverseTimer timer = new ReverseTimer
                            {
                                Message = result.Message,
                                MessageType = result.MessageType,
                                StatusCode = result.StatusCode,
                                Success = result.Success,
                                Time = result.Time - 1
                            };
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                OnReverseTimer?.Invoke(this, timer);
                            });
                        });
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    Console.WriteLine(e.StackTrace);
                }
            }
        }
        else if (message.Contains("TimerGame"))
        {
            var result = JsonSerializer.Deserialize<DurationGame>(message);
            
            if (result != null)
                OnDurationGame?.Invoke(this, result);
        }
        else if (message.Contains("PersonTime"))
        {
            var result = JsonSerializer.Deserialize<RemainingTimePerson>(message);
            
            if (result != null)
                OnRemainingTimePerson?.Invoke(this, result);
        }
        else if (message.Contains("AddPlayer"))
        {
            var result = JsonSerializer.Deserialize<AddPerson>(message);
            
            if (result != null)
                OnAddPerson?.Invoke(this, result);
        }
        else if (message.Contains("UpdateBoard"))
        {
            var result = JsonSerializer.Deserialize<UpdateBoard>(message);
            
            if (result != null)
                OnUpdateBoard?.Invoke(this, result);
        }
        else if (message.Contains("Finished"))
        {
            var result = JsonSerializer.Deserialize<FinishGame>(message);
            
            if (result != null)
                OnGameFinished?.Invoke(this, result);
        }
        else if (message.Contains("GameOver"))
        {
            var result = JsonSerializer.Deserialize<GameOverPlayer>(message);
            
            if (result != null)
                OnGameOverPlayer?.Invoke(this, result);
        }
        else if (message.Contains("PlayerIsActive"))
        {
            var result = JsonSerializer.Deserialize<PlayerIsActive>(message);
            
            if (result != null)
                OnIsActivePlayer?.Invoke(this, result);
        }
        else if (message.Contains("InActiveTime"))
        {
            var result = JsonSerializer.Deserialize<ReversTimeAnActivePlayer>(message);
            
            if (result != null)
                OnReverseTimeAnActivePlayer?.Invoke(this, result);
        }
        else if (message.Contains("RemovePlayer"))
        {
            var result = JsonSerializer.Deserialize<RemovePlayer>(message);
            
            if (result != null)
                OnRemovePlayer?.Invoke(this, result);
        }
        else if (message.Contains("UpdateColor"))
        {
            var result = JsonSerializer.Deserialize<UpdateColorPlayer>(message);
            
            if (result != null)
                OnUpdateColor?.Invoke(this, result);
        }
        else if (message.Contains("KillPiece"))
        {
            var result = JsonSerializer.Deserialize<KillAllPiece>(message);
            
            if (result != null)
                OnUpdateKillPiece?.Invoke(this, result);
        }
        else if (message.Contains("Score"))
        {
            var result = JsonSerializer.Deserialize<UpdateScore>(message);
            
            if (result != null)
                OnUpdateScore?.Invoke(this, result);
        }
    }
}