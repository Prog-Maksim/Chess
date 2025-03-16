using Backend.Enums;
using Backend.Game.Shapes;
using Backend.Models.DB;
using Backend.Services;

namespace Backend.Game.Potion.Potions;

/// <summary>
/// Зелье удвоения очков за убийство фигуры
/// </summary>
public class DoublePointsPotion: PotionBase
{
    public DoublePointsPotion(PotionEntity entity, Lazy<SendWebSocketMessage> webSocketMessage) : base(entity, webSocketMessage) {}
    
    public override async Task ApplyEffect(BaseChessGame game, ChessPlayer chessPlayer, ChessPiece? targetPiece)
    {
        if (!CheckPotionAllowed(game))
            throw new UnauthorizedAccessException("Зелья запрещены в данной игре!");
        
        if (chessPlayer.CheckUsedPotion(this))
            throw new InvalidOperationException("Данное зелье в данной игре уже использовалось!");
        
        if (game.State != GameState.InProgress)
            throw new InvalidOperationException("Сейчас игра не идет!");

        chessPlayer.DoubleScoreForNextKill = true;
        chessPlayer.AddUsedPotion(this);
    }
}