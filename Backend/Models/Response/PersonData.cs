using Backend.Enums;

namespace Backend.Models.Response;

public class PersonData
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public required string PersonId { get; set; }
    
    /// <summary>
    /// Никнейм пользователя
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// Лига пользователя
    /// </summary>
    public required string League { get; set; }
    
    /// <summary>
    /// Очки пользователя
    /// </summary>
    public int Score { get; set; }
    
    /// <summary>
    /// Уровень пользователя
    /// </summary>
    public int Level { get; set; }
    
    /// <summary>
    /// Текущее число побед
    /// </summary>
    public int NumberOfWinsLevel { get; set; }

    /// <summary>
    /// Требуемое число побед для повышения уровня
    /// </summary>
    public int RequiredNumberOfWinsLevel { get; set; }
    
    /// <summary>
    /// Разблокирован ли сундук для открытия
    /// </summary>
    public bool IsChest { get; set; }
    
    /// <summary>
    /// Зелья
    /// </summary>
    public required List<PotionData> Potions { get; set; }
}

public class PotionData
{
    /// <summary>
    /// Идентификатор зелья
    /// </summary>
    public required string PotionId { get; set; }
    
    /// <summary>
    /// Тип зелья
    /// </summary>
    public required PotionType Type { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public int Count { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool IsPurchased { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool IsUnlocked { get; set; }
}