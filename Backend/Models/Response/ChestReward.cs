using Backend.Enums;

namespace Backend.Models.Response;

public class ChestReward
{
    /// <summary>
    /// Количество полученных очков (если есть)
    /// </summary>
    public int Score { get; set; } = 0;
    
    /// <summary>
    /// Список полученных зелий (если есть)
    /// </summary>
    public PotionReward? Potion { get; set; }
    
    /// <summary>
    /// Флаг, указывающий, что выпала только награда в виде очков
    /// </summary>
    public bool IsOnlyScore;

    /// <summary>
    /// Флаг, указывающий, что выпало хотя бы одно зелье
    /// </summary>
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
    public required string PotionId { get; set; }
    
    /// <summary>
    /// Тип зелья
    /// </summary>
    public required PotionType Type { get; set; }

    /// <summary>
    /// Название зелья
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// Описание зелья
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Количество полученных экземпляров данного зелья
    /// </summary>
    public int Amount { get; set; }
}