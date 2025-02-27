using Backend.Game.Shapes;
using Backend.Services;

namespace Backend.Game;

public class ChessGame2Players: BaseChessGame
{
    public ChessGame2Players(string ownerId) : base(8, ownerId)
    {
        GameName = "Игра 2x2"; 
    }
    public ChessGame2Players(string ownerId, bool isGamePrivate) : base(8, ownerId, isGamePrivate) { }

    protected override int RequiredPlayers() => 2;
    protected override TimeSpan MaxGameTimeInSeconds() => TimeSpan.FromHours(3);
    
    
    protected override async Task HandlePlayerTimeUpdate(ChessPlayer player)
    {
        await SendWebSocketMessage.SendMessageTimerPersonTheGame(Players, player, player.RemainingTime);
    }
    
    protected override async Task InitializePlayerPieces(ChessPlayer player)
    {
        if (Players.Count == 1)
        {
            player.Color = "#FFFFFF";
            
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

    public async Task Motion(string pieceId, int newCol, int newRow)
    {
        if (newRow > 7 || newCol > 7)
            throw new ArgumentOutOfRangeException("Значение newCol или newRow должно быть в диапазоне от 0 до 7");
        
        for (int i = 0; i < Board.GetLength(0); i++)
        {
            for (int j = 0; j < Board.GetLength(1); j++)
            {
                if (Board[i, j] != null && Board[i, j].OwnerId == pieceId)
                {
                    Board[newCol, newCol] = Board[i, j];
                    Board[i, j] = null;
                }
            }
        }
    }
}