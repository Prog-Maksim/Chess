using System.Net.WebSockets;
using Backend.Enums;
using Backend.Game;
using Backend.Game.Shapes;
using Backend.Models.Response;

namespace Backend.Services;

public class GameService
{
    /// <summary>
    /// Список всех активных игр
    /// </summary>
    private List<BaseChessGame> GetAllGames { get; set; }
    private Lazy<SendWebSocketMessage> _sendWebSocketMessage;

    public delegate void DeleteGame(string gameId);
    
    public GameService(Lazy<SendWebSocketMessage> sendWebSocketMessage)
    {
        GetAllGames = new List<BaseChessGame>();
        _sendWebSocketMessage = sendWebSocketMessage;
    }

    /// <summary>
    /// Метод создания игры
    /// </summary>
    /// <param name="nameGame">Название игры</param>
    /// <param name="players">Кол-во игроков</param>
    /// <param name="personId">Идентификатор игрока</param>
    /// <param name="name">Никнейм пользователя</param>
    /// <param name="isPrivate">Приватная ли игра</param>
    /// <returns>Идентификатор игры</returns>
    public string CreateGame(string nameGame, int players, string personId, string name, bool isPrivate)
    {
        DeleteGame deleteGame = DeleteGameHandler;
        ChessPlayer player = new ChessPlayer(personId, name);

        if (players == 2)
        {
            ChessGame2Players chessGame2Players = new ChessGame2Players(nameGame, player, isPrivate, _sendWebSocketMessage, deleteGame);
            GetAllGames.Add(chessGame2Players);
            return chessGame2Players.GameId;
        }
        
        ChessGame4Players chessGame4Players = new ChessGame4Players(nameGame, player, isPrivate, _sendWebSocketMessage, deleteGame);
        GetAllGames.Add(chessGame4Players);
        return chessGame4Players.GameId;
    }

    private void DeleteGameHandler(string gameId)
    {
        var game = GetAllGames.FirstOrDefault(g => g.GameId == gameId);
        
        if (game != null)
            GetAllGames.Remove(game);
    }

    /// <summary>
    /// Оставляет заявку на вступления в игру
    /// </summary>
    /// <param name="gameId">Идентификатор игры</param>
    /// <param name="playerId">Идентификатор игрока</param>
    /// <param name="nickname">Никнейм пользователя</param>
    /// <exception cref="KeyNotFoundException">Данная игра не найдена</exception>
    public async Task<BaseResponse> JoinGame(string gameId, string playerId, string nickname)
    {
        var game = GetAllGames.Find(x => x.GameId == gameId);
        
        if (game == null)
            return new BaseResponse { Message = "Данная игра не найдена", Error = "NotFound", StatusCode = 404 };
        
        ChessPlayer player = new ChessPlayer(playerId, nickname);
        await game.RequestJoin(player);
        return new BaseResponse
        {
            StatusCode = 200,
            Success = true,
            Message = "Заявка на вступление в игру оставлена! ожидайте"
        };
    }

    /// <summary>
    /// Возвращает информацию об игре
    /// </summary>
    /// <param name="gameId">Идентификатор игры</param>
    /// <param name="playerId">Идентификатор игрока</param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException">Данная игра не найдена</exception>
    /// <exception cref="NullReferenceException">Данный игрок не найден</exception>
    /// <exception cref="UnauthorizedAccessException">У данного игрока нет доступа к игре</exception>
    public GameData GetBoard(string gameId, string playerId)
    {
        var game = GetAllGames.Find(x => x.GameId == gameId);
        
        if (game == null)
            throw new KeyNotFoundException("Данная игра не найдена");
        
        var player = game.Players.Find(x => x.Id == playerId);

        if (player == null)
            throw new NullReferenceException("Данный игрок не найден");

        if (game.IsGamePrivate && !player.IsApproved)
            throw new UnauthorizedAccessException("У вас нет доступа к этой игре");

        
        GameData data = new GameData
        {
            PersonId = playerId,
            GameId = gameId,
            GameName = game.GameName,
            GameState = game.State,

            CurrentPlayer = game.Players[game.CurrentPlayerIndex].Id,
        };
        data.Players = new List<GamePlayer>();

        foreach (var gamePlayer in game.Players)
        {
            GamePlayer gamePlayerData = new GamePlayer
            {
                PlayerId = gamePlayer.Id,
                Nickname = gamePlayer.Name,
                Time = gamePlayer.RemainingTime
            };
            data.Players.Add(gamePlayerData);
        }

        List<List<GameBoard?>> gameBoards = new();

        for (int i = 0; i < game.Board.GetLength(0); i++)
        {
            if (gameBoards.Count <= i)
                gameBoards.Add(new List<GameBoard?>());
            
            for (int j = 0; j < game.Board.GetLength(1); j++)
            {
                if (game.Board[i, j] == null)
                {
                    gameBoards[i].Add(null);
                }
                else
                {
                    var dataChessPiece = game.Board[i, j];
                    var color = game.Players.Find(p => p.Id == dataChessPiece.OwnerId).Color;
                    
                    GameBoard gameBoard = new GameBoard
                    {
                        Color = color,
                        Type = dataChessPiece.Type,
                        PieceId = dataChessPiece.ChessPieceId,
                        PersonId = dataChessPiece.OwnerId
                    };

                    gameBoards[i].Add(gameBoard);
                }
            }
        }
        data.Board = gameBoards;

        return data;
    }

    /// <summary>
    /// Выдает разрешение на вступление в игру
    /// </summary>
    /// <param name="gameId">Идентификатор игры</param>
    /// <param name="playerId">Идентификатор игрока (кому выдается разрешение)</param>
    /// <param name="sendPersonId">Кто выдает разрешение</param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException">Данная игра не найдена</exception>
    public async Task<bool> ApprovePersonTheGame(string gameId, string playerId, string sendPersonId)
    {
        var game = GetAllGames.Find(x => x.GameId == gameId);

        if (game == null)
            throw new KeyNotFoundException("Данная игра не найдена");
        
        return await game.ApprovePlayer(sendPersonId, playerId);
    }
    
    /// <summary>
    /// Запрещает пользователю в игре
    /// </summary>
    /// <param name="gameId">Идентификатор игры</param>
    /// <param name="playerId">Идентификатор игрока</param>
    /// <param name="sendPersonId">Кто запрещает в игре</param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException">Данная игра не найдена</exception>
    public async Task<bool> RejectPersonTheGame(string gameId, string playerId, string sendPersonId)
    {
        var game = GetAllGames.Find(x => x.GameId == gameId);

        if (game == null)
            throw new KeyNotFoundException("Данная игра не найдена");
        
        return await game.RejectPlayer(sendPersonId, playerId);
    }

    /// <summary>
    /// Перемещает фигуры
    /// </summary>
    /// <param name="gameId">Идентификатор игры</param>
    /// <param name="personId">Идентификатор игрока</param>
    /// <param name="oldRow">Текущий ряд фигуры</param>
    /// <param name="oldCol">Текущая строка фигуры</param>
    /// <param name="newRow">Новый ряд фигуры</param>
    /// <param name="newCol">Новая строка фигуры</param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException">Данная игра не найдена</exception>
    public async Task<bool> Moving(string gameId, string personId, int oldRow, int oldCol, int newRow, int newCol)
    {
        var game = GetAllGames.Find(x => x.GameId == gameId);

        if (game == null)
            throw new KeyNotFoundException("Данная игра не найдена");

        return await game.MakeAMove(personId, oldRow, oldCol, newRow, newCol);
    }

    /// <summary>
    /// Позволяет игроку выйти из игры
    /// </summary>
    /// <param name="gameId">Идентификатор игры</param>
    /// <param name="personId">Идентификатор игрока</param>
    /// <exception cref="KeyNotFoundException">Данная игра не найдена</exception>
    public async Task LeaveTheGame(string gameId, string personId)
    {
        var game = GetAllGames.Find(x => x.GameId == gameId);

        if (game == null)
            throw new KeyNotFoundException("Данная игра не найдена");

        game.ExitTheGame(personId);
    }

    /// <summary>
    /// Обозначает пользователя как неактивным
    /// </summary>
    /// <param name="playerId">Идентификатор игрока</param>
    public void SetUserIsInActive(string playerId)
    {
        foreach (var game in GetAllGames)
        {
            var person = game.Players.FirstOrDefault(p => p.Id == playerId);
            
            if (person != null)
                game.InActivePlayer(playerId);
        }
    }
    
    /// <summary>
    /// Обозначает пользователя как активным
    /// </summary>
    /// <param name="playerId">Идентификатор игрока</param>
    public void SetUserIsActive(string playerId)
    {
        foreach (var game in GetAllGames)
        {
            var person = game.Players.FirstOrDefault(p => p.Id == playerId);
            
            if (person != null)
                game.SetPlayerActive(playerId);
        }
    }
}