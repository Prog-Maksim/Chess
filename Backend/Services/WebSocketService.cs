using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace Backend.Services;

public class WebSocketService
{
    private readonly ConcurrentDictionary<string, WebSocket> _players;
    private readonly ILogger<WebSocketService> _logger;

    public WebSocketService(ILogger<WebSocketService> logger)
    {
        _players = new ConcurrentDictionary<string, WebSocket>();
        _logger = logger;
    }

    public async Task HandleConnectionAsync(HttpContext context, string personId)
    {
        using WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
        
        _players.TryAdd(personId, webSocket);
        _logger.LogInformation($"Игрок {personId} подключен.");

        // TODO: Реализовать оповещение друзьям что игрок в сети
        await ReceiveMessagesAsync(personId, webSocket);
    }

    public async Task ReceiveMessagesAsync(string playerId, WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await RemovePlayerAsync(playerId); // Удаляем игрока, если соединение закрыто
            }
            else
            {
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                _logger.LogInformation($"Получено сообщение от игрока {playerId}: {message}");
            }
        }
    }
    
    // Метод для удаления игрока
    public async Task RemovePlayerAsync(string playerId)
    {
        // TODO: Реализовать оповещение игрокам если пользователь отключился во время игры
        // TODO: Реализовать оповещение друзьям что игрок не в сети
        
        if (_players.TryRemove(playerId, out var webSocket))
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", CancellationToken.None);
            _logger.LogInformation($"Игрок {playerId} удалён.");
        }
        else
        {
            _logger.LogWarning($"Игрок с ID {playerId} не найден.");
        }
    }

    // Возвращает подключение игрока по его Id
    public WebSocket GetWebSocket(string playerId)
    {
        if (_players.TryGetValue(playerId, out var webSocket))
            return webSocket;

        throw new KeyNotFoundException("Данный пользователь не найден");
    }
}