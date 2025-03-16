using System.Text.Json.Serialization;

namespace Frontend.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PotionType
{
    KillPiece,
    DoublePoints,
    RandomKillPiece,
    EnlargedPiece,
    UltimateProtectionPiece
}