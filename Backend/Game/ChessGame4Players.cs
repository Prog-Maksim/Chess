using Backend.Game.Shapes;
using Backend.Services;

namespace Backend.Game;

public class ChessGame4Players : BaseChessGame
{
    public ChessGame4Players(string name, ChessPlayer player, Lazy<SendWebSocketMessage> socketMessage, GameService.DeleteGame deleteGame) : base(name, 16, player, socketMessage, deleteGame)
    {
        GameName = name; 
    }
    public ChessGame4Players(string name, ChessPlayer player, bool isGamePrivate, Lazy<SendWebSocketMessage> socketMessage, GameService.DeleteGame deleteGame) : base(name, 16, player, isGamePrivate, socketMessage, deleteGame) { }

    protected override int RequiredPlayers() => 4;
    protected override TimeSpan MaxGameTimeInSeconds() => TimeSpan.FromHours(4);
    
    protected override async Task HandlePlayerTimeUpdate(ChessPlayer player)
    {
        await WebSocketMessage.Value.SendMessageTimerPersonTheGame(Players, player, player.RemainingTime);
    }
    
    protected override async Task InitializePlayerPieces(ChessPlayer player)
    {
        if (Players.Count == 1)
            InitializePiecePerson1();
        else if (Players.Count == 3)
            InitializePiecePerson3();
        else if (Players.Count == 2)
            InitializePiecePerson2();
        else if (Players.Count == 4)
            InitializePiecePerson4();

        await SendMessageUpdateBoard();
    }

    private void InitializePiecePerson1()
    {
        var player = Players.First();
        player.Color = "#eeeeee";
            
        King king = new King(player.Id);
        AddPieceToBoard(king, 15, 7);
            
        Queen queen = new Queen(player.Id);
        AddPieceToBoard(queen, 15, 8);
            
        Bishop bishop1 = new Bishop(player.Id);
        Bishop bishop2 = new Bishop(player.Id);
        AddPieceToBoard(bishop1, 15, 6);
        AddPieceToBoard(bishop2, 15, 9);
            
        Knight knight1 = new Knight(player.Id);
        Knight knight2 = new Knight(player.Id);
        AddPieceToBoard(knight1, 15, 5);
        AddPieceToBoard(knight2, 15, 10);
            
        Rook rook1 = new Rook(player.Id);
        Rook rook2 = new Rook(player.Id);
        AddPieceToBoard(rook1, 15, 4);
        AddPieceToBoard(rook2, 15, 11);
            
        Pawn pawn1 = new Pawn(player.Id);
        Pawn pawn2 = new Pawn(player.Id);
        Pawn pawn3 = new Pawn(player.Id);
        Pawn pawn4 = new Pawn(player.Id);
        Pawn pawn5 = new Pawn(player.Id);
        Pawn pawn6 = new Pawn(player.Id);
        Pawn pawn7 = new Pawn(player.Id);
        Pawn pawn8 = new Pawn(player.Id);
        AddPieceToBoard(pawn1, 14, 4);
        AddPieceToBoard(pawn2, 14, 5);
        AddPieceToBoard(pawn3, 14, 6);
        AddPieceToBoard(pawn4, 14, 7);
        AddPieceToBoard(pawn5, 14, 8);
        AddPieceToBoard(pawn6, 14, 9);
        AddPieceToBoard(pawn7, 14, 10);
        AddPieceToBoard(pawn8, 14, 11);
    }
    private void InitializePiecePerson2()
    {
        var player = Players[1];
        player.Color = "#DD4CEE";
            
        King king = new King(player.Id);
        AddPieceToBoard(king, 7, 0);
            
        Queen queen = new Queen(player.Id);
        AddPieceToBoard(queen, 8, 0);
            
        Bishop bishop1 = new Bishop(player.Id);
        Bishop bishop2 = new Bishop(player.Id);
        AddPieceToBoard(bishop1, 6, 0);
        AddPieceToBoard(bishop2, 9, 0);
            
        Knight knight1 = new Knight(player.Id);
        Knight knight2 = new Knight(player.Id);
        AddPieceToBoard(knight1, 5, 0);
        AddPieceToBoard(knight2, 10, 0);
            
        Rook rook1 = new Rook(player.Id);
        Rook rook2 = new Rook(player.Id);
        AddPieceToBoard(rook1, 4, 0);
        AddPieceToBoard(rook2, 11, 0);
            
        Pawn pawn1 = new Pawn(player.Id);
        Pawn pawn2 = new Pawn(player.Id);
        Pawn pawn3 = new Pawn(player.Id);
        Pawn pawn4 = new Pawn(player.Id);
        Pawn pawn5 = new Pawn(player.Id);
        Pawn pawn6 = new Pawn(player.Id);
        Pawn pawn7 = new Pawn(player.Id);
        Pawn pawn8 = new Pawn(player.Id);
        AddPieceToBoard(pawn1, 4, 1);
        AddPieceToBoard(pawn2, 5, 1);
        AddPieceToBoard(pawn3, 6, 1);
        AddPieceToBoard(pawn4, 7, 1);
        AddPieceToBoard(pawn5, 8, 1);
        AddPieceToBoard(pawn6, 9, 1);
        AddPieceToBoard(pawn7, 10, 1);
        AddPieceToBoard(pawn8, 11, 1);
    }
    private void InitializePiecePerson3()
    {
        var player = Players[2];
        player.Color = "#000000";
            
        King king = new King(player.Id);
        AddPieceToBoard(king, 0, 7);
            
        Queen queen = new Queen(player.Id);
        AddPieceToBoard(queen, 0, 8);
            
        Bishop bishop1 = new Bishop(player.Id);
        Bishop bishop2 = new Bishop(player.Id);
        AddPieceToBoard(bishop1, 0, 6);
        AddPieceToBoard(bishop2, 0, 9);
            
        Knight knight1 = new Knight(player.Id);
        Knight knight2 = new Knight(player.Id);
        AddPieceToBoard(knight1, 0, 5);
        AddPieceToBoard(knight2, 0, 10);
            
        Rook rook1 = new Rook(player.Id);
        Rook rook2 = new Rook(player.Id);
        AddPieceToBoard(rook1, 0, 4);
        AddPieceToBoard(rook2, 0, 11);
            
        Pawn pawn1 = new Pawn(player.Id);
        Pawn pawn2 = new Pawn(player.Id);
        Pawn pawn3 = new Pawn(player.Id);
        Pawn pawn4 = new Pawn(player.Id);
        Pawn pawn5 = new Pawn(player.Id);
        Pawn pawn6 = new Pawn(player.Id);
        Pawn pawn7 = new Pawn(player.Id);
        Pawn pawn8 = new Pawn(player.Id);
        AddPieceToBoard(pawn1, 1, 4);
        AddPieceToBoard(pawn2, 1, 5);
        AddPieceToBoard(pawn3, 1, 6);
        AddPieceToBoard(pawn4, 1, 7);
        AddPieceToBoard(pawn5, 1, 8);
        AddPieceToBoard(pawn6, 1, 9);
        AddPieceToBoard(pawn7, 1, 10);
        AddPieceToBoard(pawn8, 1, 11);
    }
    private void InitializePiecePerson4()
    {
        var player = Players[3];
        player.Color = "#7074D5";
            
        King king = new King(player.Id);
        AddPieceToBoard(king, 7, 15);
            
        Queen queen = new Queen(player.Id);
        AddPieceToBoard(queen, 8, 15);
            
        Bishop bishop1 = new Bishop(player.Id);
        Bishop bishop2 = new Bishop(player.Id);
        AddPieceToBoard(bishop1, 6, 15);
        AddPieceToBoard(bishop2, 9, 15);
            
        Knight knight1 = new Knight(player.Id);
        Knight knight2 = new Knight(player.Id);
        AddPieceToBoard(knight1, 5, 15);
        AddPieceToBoard(knight2, 10, 15);
            
        Rook rook1 = new Rook(player.Id);
        Rook rook2 = new Rook(player.Id);
        AddPieceToBoard(rook1, 4, 15);
        AddPieceToBoard(rook2, 11, 15);
            
        Pawn pawn1 = new Pawn(player.Id);
        Pawn pawn2 = new Pawn(player.Id);
        Pawn pawn3 = new Pawn(player.Id);
        Pawn pawn4 = new Pawn(player.Id);
        Pawn pawn5 = new Pawn(player.Id);
        Pawn pawn6 = new Pawn(player.Id);
        Pawn pawn7 = new Pawn(player.Id);
        Pawn pawn8 = new Pawn(player.Id);
        AddPieceToBoard(pawn1, 4, 14);
        AddPieceToBoard(pawn2, 5, 14);
        AddPieceToBoard(pawn3, 6, 14);
        AddPieceToBoard(pawn4, 7, 14);
        AddPieceToBoard(pawn5, 8, 14);
        AddPieceToBoard(pawn6, 9, 14);
        AddPieceToBoard(pawn7, 10, 14);
        AddPieceToBoard(pawn8, 11, 14);
    }
    
    protected override async Task UpdateGameBoard()
    {
        for (int i = 0; i < Board.GetLength(0); i++)
            for (int j = 0; j < Board.GetLength(1); j++)
                Board[i, j] = null;

        for (int i = 0; i < Players.Count; i++)
        {
            if (i == 0)
                InitializePiecePerson1();
            if (i == 1)
                InitializePiecePerson2();
            if (i == 2)
                InitializePiecePerson3();
            if (i == 3)
                InitializePiecePerson4();

            _ = WebSocketMessage.Value.SendMessageUpdateColor(Players[i]);
        }
        
        await SendMessageUpdateBoard();
    }
    
    // Действия
    protected override async Task<bool> Moving(string personId, int oldRow, int oldCol, int newRow, int newCol)
    {
        if (newRow > 15 || newCol > 15 || newRow < 0 || newCol < 0)
            throw new ArgumentOutOfRangeException("Значение newCol или newRow должно быть в диапазоне от 0 до 16");
        
        var piece = Board[oldRow, oldCol];

        if (piece.OwnerId != personId)
            return false;

        var person = Players.FirstOrDefault(p => p.Id == personId);

        Board[newRow, newCol] = piece;
        Board[oldRow, oldCol] = null;

        NextTurn();
        await SendMessageUpdateBoard();
        
        return false;
    }
}