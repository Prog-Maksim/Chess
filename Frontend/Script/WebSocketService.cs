using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Frontend.Models.Request;
using Frontend.Models.WebSockerMessage;

namespace Frontend.Script;

public class WebSocketService
{
    private static WebSocketService _instance;
    private static readonly object _lock = new ();
    private static ClientWebSocket? client;

    private int _retrive;

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
        client.Options.SetRequestHeader("Authorization", $"Bearer {token}");

        try
        {
            Console.WriteLine("Connect to WebSocket...");
            await client.ConnectAsync(new Uri(Url.BaseUrlWs), CancellationToken.None);
            Console.WriteLine("WebSocket connected!");
            
            await ReceiveMessages(client);
            _retrive = 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);

            _retrive++;
            
            if (_retrive == 4)
                return;

            await ConnectAsync(token);
        }
        finally
        {
            Console.WriteLine("Close connect...");
            
            _retrive++;
            
            if (_retrive < 4)
                await ConnectAsync(token);
        }
    }

    public async Task SendMessage(MovePiece movePiece)
    {
        var message = JsonSerializer.Serialize(movePiece);
        var buffer = Encoding.UTF8.GetBytes(message);
        
        if (client != null)
            await client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, 
                CancellationToken.None);
    }
    
    private async Task ReceiveMessages(ClientWebSocket client)
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
            var result = JsonSerializer.Deserialize<UpdateBoard>(message);
            
            if (result != null)
                OnUpdateBoard?.Invoke(this, result);
        }
    }
}