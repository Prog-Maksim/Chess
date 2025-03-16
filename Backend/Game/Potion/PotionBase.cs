using Backend.Enums;
using Backend.Game.Shapes;
using Backend.Models.DB;
using Backend.Services;

namespace Backend.Game.Potion;

public abstract class PotionBase: IPotion
{
    public string Name { get; }
    public PotionType Type { get; }
    public string Description { get; }
    public int PurchasePrice { get; }
    public int UnlockPrice { get; }
    public int UnlockLevel { get; protected set; }
    
    protected readonly SendWebSocketMessage _webSocketMessage;
    
    protected PotionBase(PotionEntity entity, Lazy<SendWebSocketMessage> webSocketMessage )
    {
        Name = entity.Name;
        Type = entity.EffectType;
        Description = entity.Description;
        PurchasePrice = entity.PurchasePrice;
        UnlockPrice = entity.UnlockPrice;
        UnlockLevel = entity.UnlockLevel;
        
        _webSocketMessage = webSocketMessage.Value;
    }

    /// <summary>
    /// Применение эффекта зелья
    /// </summary>
    public abstract Task ApplyEffect(BaseChessGame game, ChessPlayer chessPlayer, ChessPiece? targetPiece);

    public virtual bool CheckPotionAllowed(BaseChessGame game)
    {
        return game.IsPotion;
    }
}