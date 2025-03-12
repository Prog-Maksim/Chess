using System.Net.WebSockets;
using System.Text;
using Backend.Enums;
using Backend.Game;
using Backend.Game.Shapes;
using Backend.Models.Response;
using Backend.Models.Response.WebSocketMessage;
using Newtonsoft.Json;

namespace Backend.Services;

public class SendWebSocketMessage
{
    private readonly WebSocketService _webSocketService;

    public SendWebSocketMessage(WebSocketService webSocketService)
    {
        _webSocketService = webSocketService;
    }
    
    /// <summary>
    /// Заявка на вступление в игру (отправляется создателю)
    /// </summary>
    /// <param name="player"></param>
    /// <param name="owner"></param>
    public async Task SendMessageJoinTheGame(ChessPlayer player, ChessPlayer owner)
    {
        JoinTheGame joinTheGame = new JoinTheGame
        {
            MessageType = "Join",
            Nickname = player.Name,
            PersonId = player.Id,
            StatusCode = 200,
            Success = true,
            Message = "Заявка на вступление в игру"
        };

        var message = JsonConvert.SerializeObject(joinTheGame);
        var buffer = Encoding.UTF8.GetBytes(message);
        
        var ws = _webSocketService.GetWebSocket(owner.Id);
        if (ws != null && ws.State == WebSocketState.Open)
        {
            await ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true,
                CancellationToken.None);
        }
        else
            Console.WriteLine("Невозможно отправить сообщение пользователю");
    }

    /// <summary>
    /// Сообщение пользователю, что ему ответили на заявку
    /// </summary>
    /// <param name="player"></param>
    /// <param name="success"></param>
    public async Task SendMessageResultJoinTheGame(ChessPlayer player, string gameId, bool success)
    {
        ResultJoinTheGame joinTheGame = new ResultJoinTheGame
        {
            MessageType = "JoinResult",
            StatusCode = 200,
            Success = true,
            Status = success,
            GameId = gameId,
            Message = "Заявка на вступление в игру " + (success ? "принята!" : "отклонена!")
        };

        var message = JsonConvert.SerializeObject(joinTheGame);
        var buffer = Encoding.UTF8.GetBytes(message);

        var ws = _webSocketService.GetWebSocket(player.Id);
        if (ws != null && ws.State == WebSocketState.Open)
        {
            await ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true,
                CancellationToken.None);
        }
        else
            Console.WriteLine($"Невозможно отправить сообщение пользователю: {player.Name}");
    }

    /// <summary>
    /// Сообщения обратный таймер до начала игры
    /// </summary>
    /// <param name="players"></param>
    /// <param name="time"></param>
    public async Task SendMessageReverseTimer(List<ChessPlayer> players, int time)
    {
        ReverseTimer reverseTimer = new ReverseTimer
        {
            MessageType = "ReverseTimer",
            StatusCode = 200,
            Success = true,
            Time = time,
            Message = $"До начала игры осталось {time}"
        };

        var message = JsonConvert.SerializeObject(reverseTimer);
        var buffer = Encoding.UTF8.GetBytes(message);

        foreach (var player in players)
        {
            var ws = _webSocketService.GetWebSocket(player.Id);
            if (ws != null && ws.State == WebSocketState.Open)
            {
                await ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true,
                    CancellationToken.None);
            }
            else
                Console.WriteLine($"Невозможно отправить сообщение пользователю: {player.Name}");
        }
    }

    /// <summary>
    /// Сообщения Таймер длительности игры
    /// </summary>
    /// <param name="players"></param>
    /// <param name="time"></param>
    public async Task SendMessageTimerGame(List<ChessPlayer> players, TimeSpan time)
    {
        DurationGame reverseTimer = new DurationGame
        {
            MessageType = "TimerGame",
            StatusCode = 200,
            Success = true,
            Time = time,
            Message = $"Игра идет {time} секунд"
        };

        var message = JsonConvert.SerializeObject(reverseTimer);
        var buffer = Encoding.UTF8.GetBytes(message);

        foreach (var player in players)
        {
            var ws = _webSocketService.GetWebSocket(player.Id);
            if (ws != null && ws.State == WebSocketState.Open)
            {
                await ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true,
                    CancellationToken.None);
            }
            else
                Console.WriteLine($"Невозможно отправить сообщение пользователю: {player.Name}");
        }
    }

    /// <summary>
    /// Сообщения Таймер оставшегося времени игры у пользователя
    /// </summary>
    /// <param name="players"></param>
    /// <param name="time"></param>
    public async Task SendMessageTimerPersonTheGame(List<ChessPlayer> players, ChessPlayer currentPlayer,
        TimeSpan time)
    {
        RemainingTimePerson reverseTimer = new RemainingTimePerson
        {
            MessageType = "PersonTime",
            StatusCode = 200,
            Success = true,
            Time = time,
            PersonId = currentPlayer.Id,
            Message = $"У игрока осталось: {time.Minutes}:{time.Seconds} времени"
        };

        var message = JsonConvert.SerializeObject(reverseTimer);
        var buffer = Encoding.UTF8.GetBytes(message);

        foreach (var player in players)
        {
            var ws = _webSocketService.GetWebSocket(player.Id);
            if (ws != null && ws.State == WebSocketState.Open)
            {
                await ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true,
                    CancellationToken.None);
            }
            else
                Console.WriteLine($"Невозможно отправить сообщение пользователю: {player.Name}");
        }
    }

    /// <summary>
    /// Сообщения о добавлении нового игрока
    /// </summary>
    /// <param name="players"></param>
    /// <param name="targetPlayer"></param>
    public async Task SendMessageAddNewPlayer(List<ChessPlayer> players, ChessPlayer targetPlayer)
    {
        AddPerson addPerson = new AddPerson
        {
            MessageType = "AddPlayer",
            StatusCode = 200,
            Success = true,
            PersonId = targetPlayer.Id,
            Nickname = targetPlayer.Name,
            Time = targetPlayer.RemainingTime,
            Message = "Добавлен новый участник"
        };

        var message = JsonConvert.SerializeObject(addPerson);
        var buffer = Encoding.UTF8.GetBytes(message);

        foreach (var player in players)
        {
            if (player.Id == targetPlayer.Id)
                continue;
            
            var ws = _webSocketService.GetWebSocket(player.Id);
            if (ws != null && ws.State == WebSocketState.Open)
            {
                await ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true,
                    CancellationToken.None);
            }
            else
                Console.WriteLine($"Невозможно отправить сообщение пользователю: {player.Name}");
        }
    }

    /// <summary>
    /// Сообщения об обновлении поля
    /// </summary>
    /// <param name="players"></param>
    public async Task SendMessageUpdateBoard(List<ChessPlayer> players, GameBoard?[,]? board)
    {
        UpdateBoard updateBoard = new UpdateBoard
        {
            MessageType = "UpdateBoard",
            StatusCode = 200,
            Success = true,
            Board = board,
            Message = "Игровое поле обновлено"
        };
        
        var message = JsonConvert.SerializeObject(updateBoard);
        var buffer = Encoding.UTF8.GetBytes(message);

        foreach (var player in players)
        {
            var ws = _webSocketService.GetWebSocket(player.Id);
            if (ws != null && ws.State == WebSocketState.Open)
            {
                await ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true,
                    CancellationToken.None);
            }
            else
                Console.WriteLine($"Невозможно отправить сообщение пользователю: {player.Name}");
        }
    }
    
    /// <summary>
    /// Сообщения о завершении игры
    /// </summary>
    /// <param name="players"></param>
    public async Task SendMessageFinishGame(ChessPlayer player, FinishGame finish)
    {
        var message = JsonConvert.SerializeObject(finish);
        var buffer = Encoding.UTF8.GetBytes(message);
        
        var ws = _webSocketService.GetWebSocket(player.Id);
        if (ws != null && ws.State == WebSocketState.Open)
        {
            await ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true,
                CancellationToken.None);
        }
        else
            Console.WriteLine($"Невозможно отправить сообщение пользователю: {player.Name}");
        
    }
    
    /// <summary>
    /// Сообщения о прогирыше игрока
    /// </summary>
    /// <param name="players"></param>
    public async Task SendMessagePlayerGameOver(List<ChessPlayer> players, string playerId)
    {
        GameOverPlayer gameOverPlayer = new GameOverPlayer
        {
            MessageType = "GameOver",
            Message = "Игрок проиграл",
            StatusCode = 200,
            Success = true,
            PersonId = playerId
        };
        
        var message = JsonConvert.SerializeObject(gameOverPlayer);
        var buffer = Encoding.UTF8.GetBytes(message);

        foreach (var player in players)
        {
            var ws = _webSocketService.GetWebSocket(player.Id);
            if (ws != null && ws.State == WebSocketState.Open)
            {
                await ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true,
                    CancellationToken.None);
            }
            else
                Console.WriteLine($"Невозможно отправить сообщение пользователю: {player.Name}");
        }
    }
    
    /// <summary>
    /// Сообщения о статусе игрока (активен, не активен)
    /// </summary>
    /// <param name="players"></param>
    public async Task SendMessageStateActivePlayer(List<ChessPlayer> players, PlayerIsActive playerIsActive)
    {
        var message = JsonConvert.SerializeObject(playerIsActive);
        var buffer = Encoding.UTF8.GetBytes(message);

        foreach (var player in players)
        {
            var ws = _webSocketService.GetWebSocket(player.Id);
            if (ws != null && ws.State == WebSocketState.Open)
            {
                await ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true,
                    CancellationToken.None);
            }
            else
                Console.WriteLine($"Невозможно отправить сообщение пользователю: {player.Name}");
        }
    }

    /// <summary>
    /// Сообщение, сколько осталось у игрока до исключения из игры
    /// </summary>
    /// <param name="players"></param>
    /// <param name="reversTimeAnActivePlayer"></param>
    public async Task SendMessageStateReverseTimeInActivePlayer(List<ChessPlayer> players, ReversTimeAnActivePlayer reversTimeAnActivePlayer)
    {
        var message = JsonConvert.SerializeObject(reversTimeAnActivePlayer);
        var buffer = Encoding.UTF8.GetBytes(message);
        
        foreach (var player in players)
        {
            try
            {
                var ws = _webSocketService.GetWebSocket(player.Id);
                if (ws != null && ws.State == WebSocketState.Open)
                {
                    await ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true,
                        CancellationToken.None);
                }
                else
                    Console.WriteLine($"Невозможно отправить сообщение пользователю: {player.Name}");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
            }
        }
    }


    /// <summary>
    /// Сообщение, Игрок исключен из игры, или выбыл
    /// </summary>
    /// <param name="players"></param>
    /// <param name="playerId"></param>
    public async Task SendMessageRemovePlayer(List<ChessPlayer> players, string playerId)
    {
        RemovePlayer removePlayer = new RemovePlayer
        {
            MessageType = "RemovePlayer",
            Message = "Игрок был удален",
            PlayerId = playerId,
            StatusCode = 200,
            Success = true
        };
        
        var message = JsonConvert.SerializeObject(removePlayer);
        var buffer = Encoding.UTF8.GetBytes(message);
        
        foreach (var player in players)
        {
            try
            {
                var ws = _webSocketService.GetWebSocket(player.Id);
                if (ws != null && ws.State == WebSocketState.Open)
                {
                    await ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true,
                        CancellationToken.None);
                }
                else
                    Console.WriteLine($"Невозможно отправить сообщение пользователю: {player.Name}");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
            }
        }
    }

    /// <summary>
    /// Сообщение, Обновление цвета фигур у игрока
    /// </summary>
    /// <param name="player"></param>
    public async Task SendMessageUpdateColor(ChessPlayer player)
    {
        UpdateColorPlayer removePlayer = new UpdateColorPlayer
        {
            MessageType = "UpdateColor",
            Message = "Ваш цвет фигур был изменен",
            StatusCode = 200,
            Color = player.Color,
            Success = true
        };
        
        var message = JsonConvert.SerializeObject(removePlayer);
        var buffer = Encoding.UTF8.GetBytes(message);
        
        try
        {
            var ws = _webSocketService.GetWebSocket(player.Id);
            if (ws != null && ws.State == WebSocketState.Open)
            {
                await ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true,
                    CancellationToken.None);
            }
            else
                Console.WriteLine($"Невозможно отправить сообщение пользователю: {player.Name}");
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e);
        }
    }

    /// <summary>
    /// Сообщение, Обновление очков у пользователя
    /// </summary>
    /// <param name="player"></param>
    public async Task SendMessageUpdateScore(ChessPlayer player, int score)
    {
        UpdateScore removePlayer = new UpdateScore
        {
            MessageType = "Score",
            Message = "Обновление очков",
            StatusCode = 200,
            Score = score,
            Success = true
        };
        
        var message = JsonConvert.SerializeObject(removePlayer);
        var buffer = Encoding.UTF8.GetBytes(message);
        
        try
        {
            var ws = _webSocketService.GetWebSocket(player.Id);
            if (ws != null && ws.State == WebSocketState.Open)
            {
                await ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true,
                    CancellationToken.None);
            }
            else
                Console.WriteLine($"Невозможно отправить сообщение пользователю: {player.Name}");
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e);
        }
    }

    /// <summary>
    /// Сообщение, Обновление какие фигуры убил пользователь
    /// </summary>
    /// <param name="player"></param>
    public async Task SendMessageUpdateKillPiece(ChessPlayer player, List<ChessPiece>? killPiece)
    {
        KillAllPiece removePlayer = new KillAllPiece
        {
            MessageType = "KillPiece",
            Message = "Обновление убитых фигур",
            StatusCode = 200,
            KillPiece = killPiece.Select(p => p.Type).ToList(),
            Success = true
        };
        
        var message = JsonConvert.SerializeObject(removePlayer);
        var buffer = Encoding.UTF8.GetBytes(message);
        
        try
        {
            var ws = _webSocketService.GetWebSocket(player.Id);
            if (ws != null && ws.State == WebSocketState.Open)
            {
                await ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true,
                    CancellationToken.None);
            }
            else
                Console.WriteLine($"Невозможно отправить сообщение пользователю: {player.Name}");
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e);
        }
    }

    /// <summary>
    /// Сообщение, Новых ход пользователя
    /// </summary>
    /// <param name="players"></param>
    /// <param name="move"></param>
    public async Task SendMessageNewMoving(List<ChessPlayer> players, Move move)
    {
        NewMove removePlayer = new NewMove
        {
            MessageType = "moving",
            Message = "Новый ход",
            Move = move,
            StatusCode = 200,
            Success = true
        };
        
        var message = JsonConvert.SerializeObject(removePlayer);
        var buffer = Encoding.UTF8.GetBytes(message);
        
        foreach (var player in players)
        {
            try
            {
                var ws = _webSocketService.GetWebSocket(player.Id);
                if (ws != null && ws.State == WebSocketState.Open)
                {
                    await ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true,
                        CancellationToken.None);
                }
                else
                    Console.WriteLine($"Невозможно отправить сообщение пользователю: {player.Name}");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
            }
        }
    }

    /// <summary>
    /// Сообщение, Новое состояние игры
    /// </summary>
    /// <param name="players"></param>
    /// <param name="gameState"></param>
    public async Task SendMessageUpdateGameState(List<ChessPlayer> players, GameState gameState)
    {
        GameStateMessage removePlayer = new GameStateMessage
        {
            MessageType = "StateGame",
            Message = "Новый ход",
            GameState = gameState,
            StatusCode = 200,
            Success = true
        };
        
        var message = JsonConvert.SerializeObject(removePlayer);
        var buffer = Encoding.UTF8.GetBytes(message);
        
        foreach (var player in players)
        {
            try
            {
                var ws = _webSocketService.GetWebSocket(player.Id);
                if (ws != null && ws.State == WebSocketState.Open)
                {
                    await ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true,
                        CancellationToken.None);
                }
                else
                    Console.WriteLine($"Невозможно отправить сообщение пользователю: {player.Name}");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
            }
        }
    }
        
}