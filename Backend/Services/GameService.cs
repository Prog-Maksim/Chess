﻿using System.Net.WebSockets;
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
    public async Task JoinGame(string gameId, string playerId, string nickname, WebSocket client)
    {
        var game = GetAllGames.Find(x => x.GameId == gameId);
        
        if (game == null)
            throw new KeyNotFoundException("Данная игра не найдена");
        
        ChessPlayer player = new ChessPlayer(playerId, nickname, client);
        await game.RequestJoin(player);
    }

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

        GameBoard?[,] gameBoards = new GameBoard?[game.Board.GetLength(0), game.Board.GetLength(1)];

        for (int i = 0; i < game.Board.GetLength(0); i++)
        {
            for (int j = 0; j < game.Board.GetLength(1); j++)
            {
                if (game.Board[i, j] == null)
                    gameBoards[i, j] = null;
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

                    gameBoards[i, j] = gameBoard;
                }
            }
        }
        data.Board = gameBoards;

        return data;
    }

    public async Task<bool> ApprovePersonTheGame(string gameId, string playerId, string sendPersonId)
    {
        var game = GetAllGames.Find(x => x.GameId == gameId);

        if (game == null)
            throw new KeyNotFoundException("Данная игра не найдена");
        
        return await game.ApprovePlayer(sendPersonId, playerId);
    }
    
    public async Task<bool> RejectPersonTheGame(string gameId, string playerId, string sendPersonId)
    {
        var game = GetAllGames.Find(x => x.GameId == gameId);

        if (game == null)
            throw new KeyNotFoundException("Данная игра не найдена");
        
        return await game.RejectPlayer(sendPersonId, playerId);
    }
}