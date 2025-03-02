﻿using Backend.Enums;
using Backend.Game.Shapes;
using Backend.Models.Response;
using Backend.Services;

namespace Backend.Game;

public class ChessGame2Players: BaseChessGame
{
    public ChessGame2Players(ChessPlayer player, Lazy<SendWebSocketMessage> socketMessage) : base(8, player, socketMessage)
    {
        GameName = "Игра 2x2"; 
    }
    public ChessGame2Players(ChessPlayer player, bool isGamePrivate, Lazy<SendWebSocketMessage> socketMessage) : base(8, player, isGamePrivate, socketMessage) { }

    protected override int RequiredPlayers() => 2;
    protected override TimeSpan MaxGameTimeInSeconds() => TimeSpan.FromHours(3);
    
    
    protected override async Task HandlePlayerTimeUpdate(ChessPlayer player)
    {
        await _webSocketMessage.Value.SendMessageTimerPersonTheGame(Players, player, player.RemainingTime);
    }
    
    protected override async Task InitializePlayerPieces(ChessPlayer player)
    {
        if (Players.Count == 1)
        {
            player.Color = "#eeeeee";
            
            King king = new King(player.Id);
            AddPieceToBoard(king, 7, 3);
            
            Queen queen = new Queen(player.Id);
            AddPieceToBoard(queen, 7, 4);
            
            Bishop bishop1 = new Bishop(player.Id);
            Bishop bishop2 = new Bishop(player.Id);
            AddPieceToBoard(bishop1, 7, 2);
            AddPieceToBoard(bishop2, 7, 5);
            
            Knight knight1 = new Knight(player.Id);
            Knight knight2 = new Knight(player.Id);
            AddPieceToBoard(knight1, 7, 1);
            AddPieceToBoard(knight2, 7, 6);
            
            Rook rook1 = new Rook(player.Id);
            Rook rook2 = new Rook(player.Id);
            AddPieceToBoard(rook1, 7, 0);
            AddPieceToBoard(rook2, 7, 7);
            
            Pawn pawn1 = new Pawn(player.Id);
            Pawn pawn2 = new Pawn(player.Id);
            Pawn pawn3 = new Pawn(player.Id);
            Pawn pawn4 = new Pawn(player.Id);
            Pawn pawn5 = new Pawn(player.Id);
            Pawn pawn6 = new Pawn(player.Id);
            Pawn pawn7 = new Pawn(player.Id);
            Pawn pawn8 = new Pawn(player.Id);
            AddPieceToBoard(pawn1, 6, 0);
            AddPieceToBoard(pawn2, 6, 1);
            AddPieceToBoard(pawn3, 6, 2);
            AddPieceToBoard(pawn4, 6, 3);
            AddPieceToBoard(pawn5, 6, 4);
            AddPieceToBoard(pawn6, 6, 5);
            AddPieceToBoard(pawn7, 6, 6);
            AddPieceToBoard(pawn8, 6, 7);
        }
        else
        {
            player.Color = "#000000";
            
            King king = new King(player.Id);
            AddPieceToBoard(king, 0, 3);
            
            Queen queen = new Queen(player.Id);
            AddPieceToBoard(queen, 0, 4);
            
            Bishop bishop1 = new Bishop(player.Id);
            Bishop bishop2 = new Bishop(player.Id);
            AddPieceToBoard(bishop1, 0, 2);
            AddPieceToBoard(bishop2, 0, 5);
            
            Knight knight1 = new Knight(player.Id);
            Knight knight2 = new Knight(player.Id);
            AddPieceToBoard(knight1, 0, 1);
            AddPieceToBoard(knight2, 0, 6);
            
            Rook rook1 = new Rook(player.Id);
            Rook rook2 = new Rook(player.Id);
            AddPieceToBoard(rook1, 0, 0);
            AddPieceToBoard(rook2, 0, 7);
            
            Pawn pawn1 = new Pawn(player.Id);
            Pawn pawn2 = new Pawn(player.Id);
            Pawn pawn3 = new Pawn(player.Id);
            Pawn pawn4 = new Pawn(player.Id);
            Pawn pawn5 = new Pawn(player.Id);
            Pawn pawn6 = new Pawn(player.Id);
            Pawn pawn7 = new Pawn(player.Id);
            Pawn pawn8 = new Pawn(player.Id);
            AddPieceToBoard(pawn1, 1, 0);
            AddPieceToBoard(pawn2, 1, 1);
            AddPieceToBoard(pawn3, 1, 2);
            AddPieceToBoard(pawn4, 1, 3);
            AddPieceToBoard(pawn5, 1, 4);
            AddPieceToBoard(pawn6, 1, 5);
            AddPieceToBoard(pawn7, 1, 6);
            AddPieceToBoard(pawn8, 1, 7);
        }

        await SendMessageUpdateBoard();
    }
    
    // Действия

    public override async Task<bool> Moving(string personId, int oldRow, int oldCol, int newRow, int newCol)
    {
        if (newRow > 7 || newCol > 7 || newRow < 0 || newCol < 0)
            throw new ArgumentOutOfRangeException("Значение newCol или newRow должно быть в диапазоне от 0 до 7");
        
        var piece = Board[oldRow, oldCol];

        if (piece.OwnerId != personId)
            return false;

        var person = Players.FirstOrDefault(p => p.Id == personId);

        if (piece.Type == PieceType.Pawn)
        {
            bool result = await ValidateMovePawn(piece, person.Color, oldRow, oldCol, newRow, newCol);
            if (result)
            {
                piece.IsFirstMove = false;
                NextTurn();
            }
            return result;
        }

        return true;
    }

    private async Task<bool> ValidateMovePawn(ChessPiece piece, string color, int oldRow, int oldCol, int newRow, int newCol)
    {
        if (oldRow == 1)
        {
            if (color == "#000000" && piece.IsFirstMove && oldCol == newCol && newRow > oldRow && (newRow - oldRow) <= 2)
            {
                Board[newRow, newCol] = piece;
                Board[oldRow, oldCol] = null;

                await SendMessageUpdateBoard();
                return true;
            }

            return false;
        }

        if (oldRow == 6)
        {
            if (color == "#eeeeee" && piece.IsFirstMove && oldCol == newCol && newRow < oldRow && (oldRow - newRow) <= 2)
            {
                Board[newRow, newCol] = piece;
                Board[oldRow, oldCol] = null;

                await SendMessageUpdateBoard();
                return true;
            }

            return false;
        }
    
        if (oldCol == newCol)
        {
            if (color == "#000000" && newRow > oldRow && (newRow - oldRow) <= 1)
            {
                Board[newRow, newCol] = piece;
                Board[oldRow, oldCol] = null;
                
                await SendMessageUpdateBoard();
                return true;
            }
            if (color == "#eeeeee" && newRow < oldRow && (oldRow - newRow) <= 1)
            {
                Board[newRow, newCol] = piece;
                Board[oldRow, oldCol] = null;
                
                await SendMessageUpdateBoard();
                return true;
            }

            return false;
        }

        return false;
    }
}