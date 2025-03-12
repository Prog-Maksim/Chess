using Backend.Enums;
using Backend.Game.GameModes;
using Backend.Game.Shapes;
using Backend.Repository.Interfaces;
using Backend.Services;

namespace Backend.Game;

public class ChessGame2Players: BaseChessGame
{
    public ChessGame2Players(string name, ChessPlayer player, IGameMode mode, Lazy<SendWebSocketMessage> socketMessage, GameService.DeleteGame deleteGame, IUserDataRepository userDataRepository) : base(name, 8, mode, player, socketMessage, deleteGame, userDataRepository)
    {
        GameName = "Игра 2x2"; 
    }
    public ChessGame2Players(string name, ChessPlayer player, IGameMode mode, bool isGamePrivate, Lazy<SendWebSocketMessage> socketMessage, GameService.DeleteGame deleteGame, IUserDataRepository userDataRepository) : base(name, 8, mode, player, isGamePrivate, socketMessage, deleteGame, userDataRepository) { }

    protected override int RequiredPlayers() => 2;
    
    protected override async Task HandlePlayerTimeUpdate(ChessPlayer player, TimeSpan time)
    {
        await WebSocketMessage.Value.SendMessageTimerPersonTheGame(Players, player, time);
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

    protected override async Task UpdateGameBoard()
    {
        var players = Players.Where(p =>
            p.State != PlayerState.Active && p.State != PlayerState.Stopped && p.State != PlayerState.InActive).ToList();
        
        for (int i = 0; i < Board.GetLength(0); i++)
        {
            for (int j = 0; j < Board.GetLength(1); j++)
            {
                if (Board[i, j] != null && players.Select(p => p.Id).Contains(Board[i, j].OwnerId))
                {
                    Board[i, j] = null;
                }
            }
        }
        
        await SendMessageUpdateBoard();
    }
    
    // Действия
    protected override async Task<bool> Moving(string personId, int oldRow, int oldCol, int newRow, int newCol)
    {
        if (newRow > 7 || newCol > 7 || newRow < 0 || newCol < 0)
            throw new ArgumentOutOfRangeException("Значение newCol или newRow должно быть в диапазоне от 0 до 8");
        
        var piece = Board[oldRow, oldCol];

        if (piece == null)
            return false;

        if (piece.OwnerId != personId)
            return false;

        var person = Players.FirstOrDefault(p => p.Id == personId);
        if (piece.OwnerId != person.Id)
            return false;

        if (Board[newRow, newCol] != null)
        {
            person.Score += Board[newRow, newCol].Score;
            await person.AddKillPiece(Board[newRow, newCol]);
        }
        
        Board[newRow, newCol] = piece;
        Board[oldRow, oldCol] = null;

        NextTurn();
        await SendMessageUpdateBoard();
        await AddNewMoveAsync(personId, oldRow, oldCol, newRow, newCol, person.GetRemainingTime());
        
        return true;
    }
}