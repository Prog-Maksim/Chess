using Frontend.Enums;

namespace Frontend.Models;

public class PotionAvailable
{
    public required List<PotionDataAvailable> Potions { get; set; }
}

public class PotionDataAvailable
{
    public PotionType Type { get; set; }
    public required string PotionId { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsUnlocked { get; set; }
}