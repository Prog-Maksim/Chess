using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend.Models.DB;

public class UserPotionInventory
{
    /// <summary>
    /// Уникальный идентификатор записи
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    
    /// <summary>
    /// Идентификатор игрока
    /// </summary>
    public required string PersonId { get; set; }
    
    /// <summary>
    /// Идентификатор зелья
    /// </summary>
    public required string PotionId { get; set; }
    
    /// <summary>
    /// Кол-во зелья
    /// </summary>
    public int Quantity { get; set; }
}