using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Backend.Models.Request;

namespace Backend.Services;

public class WebSocketService
{
    private readonly Dictionary<string, WebSocket> _players;
    private readonly ILogger<WebSocketService> _logger;
    private readonly Lazy<GameService> _gameService;

    public WebSocketService(ILogger<WebSocketService> logger, Lazy<GameService> gameService)
    {
        _players = new Dictionary<string, WebSocket>();
        _logger = logger;
        _gameService = gameService;
    }

    public async Task HandleConnectionAsync(WebSocket webSocket, string personId)
    {
        _players[personId] = webSocket;
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
                await ParseMessage(message);
            }
        }
        await RemovePlayerAsync(playerId);
    }

    public async Task ParseMessage(string message)
    {
        if (message.Contains("MovePiece"))
        {
            var result = JsonSerializer.Deserialize<MovePiece>(message);
            var dataToken = JwtService.GetJwtTokenData(result.token);
            
            await _gameService.Value.Moving(result.gameId, dataToken.PersonId, result.FromRow, result.FromCol, result.ToRow, result.ToCol);
        }
    }
    
    // Метод для удаления игрока
    public async Task RemovePlayerAsync(string playerId)
    {
        // TODO: Реализовать оповещение игрокам если пользователь отключился во время игры
        // TODO: Реализовать оповещение друзьям что игрок не в сети
        
        _players.Remove(playerId);
        _logger.LogInformation($"Игрок {playerId} удалён.");
    }

    // Возвращает подключение игрока по его Id
    public WebSocket GetWebSocket(string playerId)
    {
        if (_players.TryGetValue(playerId, out var webSocket))
            return webSocket;

        throw new KeyNotFoundException("Данный пользователь не найден");
    }
}