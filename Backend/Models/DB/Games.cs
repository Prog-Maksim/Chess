using System.Text.Json.Serialization;
using Backend.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend.Models.DB;

public class Games
{
    /// <summary>
    /// Уникальный идентификатор записи
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    
    /// <summary>
    /// Название игры
    /// </summary>
    public required string Title { get; set; }
    
    /// <summary>
    /// Тип игры
    /// </summary>
    public GameMode GameMode { get; set; }
    
    /// <summary>
    /// Идентификатор победителя
    /// </summary>
    public required string WinePersonId { get; set; }
    
    /// <summary>
    /// Приватная ли игра
    /// </summary>
    public bool IsPrivate { get; set; }
    
    /// <summary>
    /// Доступны ли в игре зелья
    /// </summary>
    public bool IsPotion { get; set; }
    
    /// <summary>
    /// Длительность игры
    /// </summary>
    public TimeSpan DurationGame { get; set; }
    
    /// <summary>
    /// Дата и время создания игры
    /// </summary>
    public DateTime DateCreated { get; set; }
    
    /// <summary>
    /// Список участников
    /// </summary>
    public required List<string> ListParticipants { get; set; }
    
    /// <summary>
    /// Ходы пользователей
    /// </summary>
    public required List<Moves> Moves { get; set; }
}

public class Moves
{
    /// <summary>
    /// Идентификатор игра
    /// </summary>
    public required string PlayerId { get; set; }
    
    /// <summary>
    /// Начальная позиция фигуры
    /// </summary>
    public int StartRow { get; set; }
    
    /// <summary>
    /// Начальная позиция фигуры
    /// </summary>
    public int StartColumn { get; set; }
    
    /// <summary>
    /// Конечная позиция фигуры
    /// </summary>
    public int EndRow { get; set; }
    
    /// <summary>
    /// Конечная позиция фигуры
    /// </summary>
    public int EndColumn { get; set; }
    
    /// <summary>
    /// Длительность хода
    /// </summary>
    public TimeSpan DurationMoves { get; set; }
}