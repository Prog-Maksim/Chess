using System.Text.Json.Serialization;

namespace Frontend.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GameMode
{
    Classic,
    Rapid,
    Blitz,
    Bullet
}