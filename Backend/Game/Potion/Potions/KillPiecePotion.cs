using Backend.Enums;
using Backend.Game.Shapes;
using Backend.Models.DB;
using Backend.Services;

namespace Backend.Game.Potion.Potions;

/// <summary>
/// Зелье убийства фигуры на поле
/// </summary>
public class KillPiecePotion: PotionBase
{
    public KillPiecePotion(PotionEntity entity, Lazy<SendWebSocketMessage> webSocketMessage) : base(entity, webSocketMessage) {}
    
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
        
        if (targetPiece.Type == PieceType.King)
            throw new InvalidOperationException("Короля нельзя убить");
        
        if (targetPiece.OwnerId == chessPlayer.Id)
            throw new InvalidOperationException("Зелье можно использовать только на чужие фигуры!");
        
        chessPlayer.Score += targetPiece.Score;
        await chessPlayer.AddKillPiece(targetPiece);
        
        
        game.RemovePiece(targetPiece.Row, targetPiece.Column);
        await game.SendMessageUpdateBoard();
        await _webSocketMessage.SendMessageUsePotion(game.Players, this, chessPlayer);
        chessPlayer.AddUsedPotion(this);
        game.NextTurn();
    }
}