using System.Net.WebSockets;
using System.Text;
using Backend.Game;
using Backend.Models.Response;
using Backend.Models.Response.WebSockerMessage;
using Newtonsoft.Json;

namespace Backend.Services;

public static class SendWebSocketMessage
{
    /// <summary>
    /// Заявка на вступление в игру (отправляется создателю)
    /// </summary>
    /// <param name="player"></param>
    /// <param name="owner"></param>
    public static async Task SendMessageJoinTheGame(ChessPlayer player, ChessPlayer owner)
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

        if (owner.Client != null && owner.Client.State == WebSocketState.Open)
        {
            await owner.Client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true,
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
    public static async Task SendMessageResultJoinTheGame(ChessPlayer player, bool success)
    {
        ResultJoinTheGame joinTheGame = new ResultJoinTheGame
        {
            MessageType = "JoinResult",
            StatusCode = 200,
            Success = true,
            Status = success,
            Message = "Заявка на вступление в игру " + (success ? "принята!" : "отклонена!")
        };

        var message = JsonConvert.SerializeObject(joinTheGame);
        var buffer = Encoding.UTF8.GetBytes(message);

        if (player.Client != null && player.Client.State == WebSocketState.Open)
        {
            await player.Client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true,
                CancellationToken.None);
        }
        else
            Console.WriteLine("Невозможно отправить сообщение пользователю");
    }

    /// <summary>
    /// Сообщения обратный таймер до начала игры
    /// </summary>
    /// <param name="players"></param>
    /// <param name="time"></param>
    public static async Task SendMessageReverseTimer(List<ChessPlayer> players, int time)
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
            if (player.Client != null && player.Client.State == WebSocketState.Open)
            {
                await player.Client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true,
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
    public static async Task SendMessageTimerGame(List<ChessPlayer> players, TimeSpan time)
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
            if (player.Client != null && player.Client.State == WebSocketState.Open)
            {
                await player.Client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true,
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
    public static async Task SendMessageTimerPersonTheGame(List<ChessPlayer> players, ChessPlayer currentPlayer,
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
            if (player.Client != null && player.Client.State == WebSocketState.Open)
            {
                await player.Client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true,
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
    public static async Task SendMessageAddNewPlayer(List<ChessPlayer> players, ChessPlayer targetPlayer)
    {
        AddPerson addPerson = new AddPerson
        {
            MessageType = "AddPlayer",
            StatusCode = 200,
            Success = true,
            PersonId = targetPlayer.Id,
            Nickname = targetPlayer.Name,
            Message = "Добавлен новый участник"
        };

        var message = JsonConvert.SerializeObject(addPerson);
        var buffer = Encoding.UTF8.GetBytes(message);

        foreach (var player in players)
        {
            if (player.Id == targetPlayer.Id)
                continue;
            
            if (player.Client != null && player.Client.State == WebSocketState.Open)
            {
                await player.Client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true,
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
    public static async Task SendMessageUpdateBoard(List<ChessPlayer> players, GameBoard?[,]? board)
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
            if (player.Client != null && player.Client.State == WebSocketState.Open)
            {
                await player.Client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true,
                    CancellationToken.None);
            }
            else
                Console.WriteLine($"Невозможно отправить сообщение пользователю: {player.Name}");
        }
    }
}