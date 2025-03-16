using Backend.Enums;

namespace Backend.Game.GameModes.Blitz;

public class Blitz2Player: IGameMode
{
    public GameMode GameMode { get; set; } = GameMode.Blitz;
    public TimeSpan GetPlayerTimeDuration() => TimeSpan.FromMinutes(8);
    public TimeSpan GetIncrementTime() => TimeSpan.FromSeconds(3);
}