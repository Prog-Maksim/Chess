using System.Text.Json.Serialization;

namespace Frontend.Models;

public class PersonData
{
    [JsonPropertyName("personId")]
    public required string PersonId { get; set; }
    
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    
    [JsonPropertyName("league")]
    public required string League { get; set; }
    
    [JsonPropertyName("score")]
    public int Score { get; set; }
    
    [JsonPropertyName("level")]
    public int Level { get; set; }
    
    [JsonPropertyName("potions")]
    public List<PotionData>? Potions { get; set; }
}

public class PotionData
{
    /// <summary>
    /// Идентификатор зелья
    /// </summary>
    [JsonPropertyName("potionId")]
    public required string PotionId { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("isPurchased")]
    public bool IsPurchased { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("isUnlocked")]
    public bool IsUnlocked { get; set; }
}