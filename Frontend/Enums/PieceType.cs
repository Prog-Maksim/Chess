using System.Text.Json.Serialization;

namespace Frontend.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PieceType
{
    King,
    Queen,
    Rook,
    Bishop,
    Knight,
    Pawn
}