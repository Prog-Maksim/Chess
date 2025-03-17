using System.Text.Json.Serialization;
using Backend.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend.Models.DB;

public class Person
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
    /// Никнейм пользователя
    /// </summary>
    public required string Nickname { get; set; }
    
    /// <summary>
    /// Почта пользователя
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Версия пароля
    /// </summary>
    public required int PasswordVersion { get; set; } = 1;
    
    /// <summary>
    /// Статус игрока
    /// </summary>
    public bool IsBanned { get; set; } = false;

    /// <summary>
    /// Роль игрока
    /// </summary>
    public PersonRole Role { get; set; } = PersonRole.Player;
    
    /// <summary>
    /// Пароль пользователя
    /// </summary>
    public string? Password { get; set; }
}