using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend.Models.DB;

public class UserData
{
    /// <summary>
    /// Уникальный идентификатор записи
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public required string PersonId { get; set; }
    
    /// <summary>
    /// Кол-во очков у пользователя
    /// </summary>
    public int Score { get; set; }
    
    /// <summary>
    /// Разблокированные пользователем зелья
    /// </summary>
    public List<string>? UnlockedPotions { get; set; }
    
    /// <summary>
    /// Кол-во сыгранных игр
    /// </summary>
    public int GamesPlayerd {get; set;}
}