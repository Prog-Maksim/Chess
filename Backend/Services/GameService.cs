using System.Net.WebSockets;
using Backend.Game;

namespace Backend.Services;

public class GameService
{
    /// <summary>
    /// Список всех активных игр
    /// </summary>
    private List<BaseChessGame> GetAllGames { get; set; }
    
    public GameService()
    {
        GetAllGames = new List<BaseChessGame>();
    }

    /// <summary>
    /// Метод создания игры на 2 игрока
    /// </summary>
    /// <param name="personId">Идентификатор игрока</param>
    /// <param name="name">Никнейм пользователя</param>
    /// <returns>Идентификатор игры</returns>
    public string CreateGame2Players(string personId, string name)
    {
        ChessGame2Players chessGame2Players = new ChessGame2Players(personId);
        GetAllGames.Add(chessGame2Players);

        return chessGame2Players.GameId;
    }

    /// <summary>
    /// Оставляет заявку на вступления в игру
    /// </summary>
    /// <param name="gameId">Идентификатор игры</param>
    /// <param name="playerId">Идентификатор игрока</param>
    /// <param name="nickname">Никнейм пользователя</param>
    /// <param name="client">WebSocket игрока</param>
    /// <exception cref="KeyNotFoundException">Данная игра не найдена</exception>
    public void JoinGame(string gameId, string playerId, string nickname, WebSocket client)
    {
        var game = GetAllGames.Find(x => x.GameId == gameId);
        
        if (game == null)
            throw new KeyNotFoundException("Данная игра не найдена");
        
        ChessPlayer player = new ChessPlayer(playerId, nickname, client);
        game.RequestJoin(player);
    }
}