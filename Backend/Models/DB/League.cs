using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend.Models.DB;

public class League
{
    /// <summary>
    /// Уникальный идентификатор записи
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    
    /// <summary>
    /// Идентификатор лиги
    /// </summary>
    public int LeagueId { get; set; }
    
    /// <summary>
    /// Базовое кол-во очков за победу или поражение
    /// </summary>
    public int RatingIsWinner { get; set; }
    
    /// <summary>
    /// Название лиги
    /// </summary>
    public required string LeagueName { get; set; }
    
    /// <summary>
    /// Минимальное кол-во рейтинга для лиги
    /// </summary>
    public int MinRating { get; set; }
    
    /// <summary>
    /// Максимальное кол-во рейтинга для лиги
    /// </summary>
    public int MaxRating { get; set; }
    
}