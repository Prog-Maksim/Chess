using Backend.Enums;

namespace Backend.Models.Response;

public class PersonData
{
    public required string PersonId { get; set; }
    public required string Name { get; set; }
    public required string League { get; set; }
    
    public int Score { get; set; }
    public int Level { get; set; }
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