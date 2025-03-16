using Backend.Enums;
using Backend.Game.Shapes;
using Backend.Models.DB;
using Backend.Services;

namespace Backend.Game.Potion.Potions;

/// <summary>
/// Рандомное убийство фигуры
/// </summary>
public class RandomKillPotion: PotionBase
{
    public RandomKillPotion(PotionEntity entity, Lazy<SendWebSocketMessage> webSocketMessage) : base(entity, webSocketMessage) {}
    
    public override async Task ApplyEffect(BaseChessGame game, ChessPlayer chessPlayer, ChessPiece? targetPiece)
    {
        if (!CheckPotionAllowed(game))
            throw new UnauthorizedAccessException("Зелья запрещены в данной игре!");
        
        if (chessPlayer.CheckUsedPotion(this))
            throw new InvalidOperationException("Данное зелье в данной игре уже использовалось!");
        
        if (game.State != GameState.InProgress)
            throw new InvalidOperationException("Сейчас игра не идет!");
        
        if (game.Players[game.CurrentPlayerIndex].Id != chessPlayer.Id)
            throw new InvalidOperationException("Сейчас не ваш ход!");
        
        var enemyPieces = new List<ChessPiece>();

        for (int row = 0; row < game.Board.GetLength(0); row++)
        {
            for (int col = 0; col < game.Board.GetLength(1); col++)
            {
                var piece = game.Board[row, col];
                
                if (piece != null && piece.OwnerId != chessPlayer.Id && piece.Type != PieceType.King)
                {
                    enemyPieces.Add(piece);
                }
            }
        }

        Random random = new Random();
        if (enemyPieces.Any())
        {
            var pieceToKill = enemyPieces[random.Next(enemyPieces.Count)];
            
            chessPlayer.Score += pieceToKill.Score;
            await chessPlayer.AddKillPiece(pieceToKill);
            game.RemovePiece(pieceToKill.Row, pieceToKill.Column);
            
            // шанс убийства дополнительной рядом стоящей фигуры 25%
            if (random.NextDouble() < 0.25)
            {
                var adjacentPieces = GetAdjacentPieces(game, pieceToKill);
                if (adjacentPieces.Any())
                {
                    var randomAdjacent = adjacentPieces[random.Next(adjacentPieces.Count)];
                    
                    chessPlayer.Score += randomAdjacent.Score;
                    await chessPlayer.AddKillPiece(randomAdjacent);
                    game.RemovePiece(randomAdjacent.Row, randomAdjacent.Column);
                }
            }
        }
        
        await game.SendMessageUpdateBoard();
        await _webSocketMessage.SendMessageUsePotion(game.Players, this, chessPlayer);
        chessPlayer.AddUsedPotion(this);
        game.NextTurn();
    }
    
    private List<ChessPiece> GetAdjacentPieces(BaseChessGame game, ChessPiece piece)
    {
        var adjacentPieces = new List<ChessPiece>();

        for (int row = Math.Max(0, piece.Row - 1); row <= Math.Min(game.Board.GetLength(0) - 1, piece.Row + 1); row++)
        {
            for (int col = Math.Max(0, piece.Column - 1); col <= Math.Min(game.Board.GetLength(1) - 1, piece.Column + 1); col++)
            {
                var adjacentPiece = game.Board[row, col];

                if (adjacentPiece != null && adjacentPiece != piece && adjacentPiece.Type != PieceType.King)
                {
                    adjacentPieces.Add(adjacentPiece);
                }
            }
        }

        return adjacentPieces;
    }
}