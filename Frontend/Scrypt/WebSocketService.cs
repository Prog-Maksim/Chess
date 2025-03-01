using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Windows;
using Frontend.Models.WebSockerMessage;

namespace Frontend.Scrypt;

public class WebSocketService
{
    private static WebSocketService _instance;
    private static readonly object _lock = new object();
    private static ClientWebSocket? client;

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
        if (client != null)
            return;
        
        client = new ClientWebSocket();
        var url = "wss://monitor-advanced-suddenly.ngrok-free.app/api/v1/Game/connect";
        
        client.Options.SetRequestHeader("Authorization", $"Bearer {token}");

        try
        {
            Console.WriteLine("Connect to WebSocket...");
            await client.ConnectAsync(new Uri(url), CancellationToken.None);
            Console.WriteLine("WebSocket connected!");
            
            await ReceiveMessages(client);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
        finally
        {
            Console.WriteLine("Close connect...");
        }
    }
    private async Task ReceiveMessages(ClientWebSocket client)
    {
        var buffer = new byte[1024];

        while (client.State == WebSocketState.Open)
        {
            var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Сервер закрыл соединение", CancellationToken.None);
                Console.WriteLine("1: Сервер закрыл соединение.");
                break;
            }

            string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Console.WriteLine($"Sever: {message}");
            ParseMessages(message);
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
    

    private void ParseMessages(string message)
    {
        if (message.Contains("Join"))
        {
        //     var result = JsonSerializer.Deserialize<JoinTheGame>(message);
        //     
        //     if (result != null)
        //         OnJoinTheGame?.Invoke(this, result);
        }
        if (message.Contains("JoinResult"))
        {
            var result = JsonSerializer.Deserialize<ResultJoinTheGame>(message);
            
            if (result != null)
                OnResultJoinTheGame?.Invoke(this, result);
        }
        else if (message.Contains("ReverseTimer"))
        {
            var result = JsonSerializer.Deserialize<ReverseTimer>(message);
            
            if (result != null)
                OnReverseTimer?.Invoke(this, result);
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
            // var result = JsonSerializer.Deserialize<UpdateBoard>(message);
            //
            // if (result != null)
            //     OnUpdateBoard?.Invoke(this, result);
        }
    }
}