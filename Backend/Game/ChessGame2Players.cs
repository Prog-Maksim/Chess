namespace Backend.Game;

public class ChessGame2Players: BaseChessGame
{
    public ChessGame2Players(string ownerId) : base(8, ownerId) { }
    public ChessGame2Players(string ownerId, bool isGamePrivate) : base(8, ownerId, isGamePrivate) { }

    protected override int RequiredPlayers() => 2;
    protected override int MaxGameTimeInSeconds() => 3600; // 1 час
    
    protected override void InitializeBoard()
    {
        
    }
    
    protected override void HandlePlayerTimeUpdate(ChessPlayer player)
    {
        
    }

    protected override void InitializePlayerPieces(ChessPlayer player)
    {
        
    }
}