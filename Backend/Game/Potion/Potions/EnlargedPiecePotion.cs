using Backend.Enums;
using Backend.Game.Shapes;
using Backend.Models.DB;
using Backend.Services;

namespace Backend.Game.Potion.Potions;

/// <summary>
/// Увеличивает фигуру в размере
/// </summary>
public class EnlargedPiecePotion: PotionBase
{
    public EnlargedPiecePotion(PotionEntity entity, Lazy<SendWebSocketMessage> webSocketMessage) : base(entity, webSocketMessage) {}
    
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
        
        if (targetPiece == null)
            throw new ArgumentNullException(nameof(targetPiece));
        
        if (targetPiece.OwnerId != chessPlayer.Id)
            throw new InvalidOperationException("Зелье нельзя использовать на чужие фигуры!");
        
        if (targetPiece is EnlargedPieceClone)
            throw new InvalidOperationException("Нельзя увеличить фигуру-клон.");
        
        var board = game.Board;
        int row = targetPiece.Row;
        int col = targetPiece.Column;
        
        // возможные направления
        var directions = new []
        {
            (-1, -1), // Влево-вверх
            (-1, 1),  // Вправо-вверх
            (1, -1),  // Влево-вниз
            (1, 1)    // Вправо-вниз
        };
        
        foreach (var (dr, dc) in directions)
        {
            int r2 = row + dr, c2 = col;    // Вертикальная копия
            int r3 = row, c3 = col + dc;    // Горизонтальная копия
            int r4 = row + dr, c4 = col + dc; // Диагональная копия
            
            if (IsCellFree(board, r2, c2) && IsCellFree(board, r3, c3) && IsCellFree(board, r4, c4))
            {
                targetPiece.IsOriginalEnlarged = true; // Флаг оригинальной фигуры
                targetPiece.IsEnlarged = true;
                
                // Создаем "копии" фигуры
                board[r2, c2] = new EnlargedPieceClone(targetPiece, r2, c2, targetPiece.Type, targetPiece.OwnerId, targetPiece.Score);
                board[r3, c3] = new EnlargedPieceClone(targetPiece, r3, c3, targetPiece.Type, targetPiece.OwnerId, targetPiece.Score);
                board[r4, c4] = new EnlargedPieceClone(targetPiece, r4, c4, targetPiece.Type, targetPiece.OwnerId, targetPiece.Score);
                
                await game.SendMessageUpdateBoard();
                await _webSocketMessage.SendMessageUsePotion(game.Players, this, chessPlayer);
                chessPlayer.AddUsedPotion(this);
                game.NextTurn();
                return;
            }
        }
        
        throw new InvalidOperationException("Невозможно увеличить фигуру: нет доступного пространства.");
    }
    
    private bool IsCellFree(ChessPiece?[,] board, int row, int col)
    {
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);

        return row >= 0 && row < rows && col >= 0 && col < cols && board[row, col] == null;
    }
}