using System.Text.Json.Serialization;
using Backend.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend.Models.DB;

public class PotionEntity
{
    /// <summary>
    /// Уникальный идентификатор записи
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    
    /// <summary>
    /// Идентификатор зелья
    /// </summary>
    public required string PotionId { get; set; }
    
    /// <summary>
    /// Название зелья
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// Описание зелья
    /// </summary>
    public required string Description { get; set; }
    
    /// <summary>
    /// Цена покупки зелья
    /// </summary>
    public int PurchasePrice { get; set; }
    
    /// <summary>
    /// Цена разблокировки зелья
    /// </summary>
    public int UnlockPrice { get; set; }
    
    /// <summary>
    /// Тиа эффекта
    /// </summary>
    public PotionType EffectType { get; set; }
    
    /// <summary>
    /// Минимальный уровень игрока для разблокировки
    /// </summary>
    public int UnlockLevel { get; set; }
}