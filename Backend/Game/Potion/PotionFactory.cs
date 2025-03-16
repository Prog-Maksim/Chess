using Backend.Enums;
using Backend.Game.Potion.Potions;
using Backend.Models.DB;
using Backend.Services;

namespace Backend.Game.Potion;

public static class PotionFactory
{
    private static Lazy<SendWebSocketMessage> _webSocketMessage;
    
    public static void Initialize(Lazy<SendWebSocketMessage> webSocketMessage)
    {
        _webSocketMessage = webSocketMessage;
    }
    
    public static IPotion CreatePotion(PotionEntity entity)
    {
        return entity.EffectType switch
        {
            PotionType.KillPiece => new KillPiecePotion(entity, _webSocketMessage),
            PotionType.DoublePoints => new DoublePointsPotion(entity, _webSocketMessage),
            PotionType.RandomKillPiece => new RandomKillPotion(entity, _webSocketMessage),
            PotionType.EnlargedPiece => new EnlargedPiecePotion(entity, _webSocketMessage),
            PotionType.UltimateProtectionPiece => new UltimateProtectionPotion(entity, _webSocketMessage),
            _ => throw new ArgumentException($"Неизвестный тип зелья: {entity.EffectType}")
        };
    }
}