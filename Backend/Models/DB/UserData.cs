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

    /// <summary>
    /// Кол-во выйгранных игр
    /// </summary>
    public int NumberOfWins { get; set; } = 0;

    /// <summary>
    /// Лига игрока
    /// </summary>
    public string League { get; set; } = "Пешечник";

    /// <summary>
    /// Рейтинг
    /// </summary>
    public int Rating { get; set; } = 0;
    
    /// <summary>
    /// Уровень пользователя
    /// </summary>
    public int Level { get; set; } = 1;

    /// <summary>
    /// Доступен ли сундук для открытия
    /// </summary>
    public bool IsChest { get; set; } = false;
    
    /// <summary>
    /// Кол-во выйгранных игр для уровня
    /// </summary>
    public int NumberOfWinsLevel { get; set; } = 0;
    
}