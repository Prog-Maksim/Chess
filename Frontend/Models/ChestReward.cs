using System.Text.Json.Serialization;
using Frontend.Enums;

namespace Backend.Models.Response;

public class ChestReward
{
    /// <summary>
    /// Количество полученных очков (если есть)
    /// </summary>
    [JsonPropertyName("score")]
    public int Score { get; set; } = 0;
    
    /// <summary>
    /// Список полученных зелий (если есть)
    /// </summary>
    [JsonPropertyName("potion")]
    public PotionReward? Potion { get; set; }
    
    /// <summary>
    /// Флаг, указывающий, что выпала только награда в виде очков
    /// </summary>
    [JsonPropertyName("isOnlyScore")]
    public bool IsOnlyScore;

    /// <summary>
    /// Флаг, указывающий, что выпало хотя бы одно зелье
    /// </summary>
    [JsonPropertyName("isPotionRewarded")]
    public bool IsPotionRewarded;
}

/// <summary>
/// Структура, представляющая полученное зелье
/// </summary>
public class PotionReward
{
    /// <summary>
    /// Идентификатор зелья
    /// </summary>
    [JsonPropertyName("potionId")]
    public required string PotionId { get; set; }
    
    /// <summary>
    /// Тип зелья
    /// </summary>
    [JsonPropertyName("type")]
    public required PotionType Type { get; set; }

    /// <summary>
    /// Название зелья
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    
    /// <summary>
    /// Описание зелья
    /// </summary>
    [JsonPropertyName("description")]
    public required string Description { get; set; }

    /// <summary>
    /// Количество полученных экземпляров данного зелья
    /// </summary>
    [JsonPropertyName("amount")]
    public int Amount { get; set; }
}