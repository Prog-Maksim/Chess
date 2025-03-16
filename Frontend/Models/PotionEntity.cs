using System.Text.Json.Serialization;
using Frontend.Enums;

namespace Frontend.Models;

public class PotionEntity
{
    /// <summary>
    /// Идентификатор зелья
    /// </summary>
    [JsonPropertyName("potionId")]
    public required string PotionId { get; set; }
    
    
    /// <summary>
    /// Название зелья
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    
    /// <summary>
    /// Описание зелья
    /// </summary>
    [JsonPropertyName("description")]
    public required string Description { get; set; }
    
    /// <summary>
    /// Цена покупки зелья
    /// </summary>
    [JsonPropertyName("purchasePrice")]
    public int PurchasePrice { get; set; }
    
    /// <summary>
    /// Цена разблокировки зелья
    /// </summary>
    [JsonPropertyName("unlockPrice")]
    public int UnlockPrice { get; set; }
    
    /// <summary>
    /// Тиа эффекта
    /// </summary>
    [JsonPropertyName("effectType")]
    public PotionType EffectType { get; set; }
    
    /// <summary>
    /// Минимальный уровень игрока для разблокировки
    /// </summary>
    [JsonPropertyName("unlockLevel")]
    public int UnlockLevel { get; set; }
}