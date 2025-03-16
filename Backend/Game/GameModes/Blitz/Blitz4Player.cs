using Backend.Enums;

namespace Backend.Game.GameModes.Blitz;

public class Blitz4Player: IGameMode
{
    public GameMode GameMode { get; set; } = GameMode.Blitz;
    public TimeSpan GetPlayerTimeDuration() => TimeSpan.FromMinutes(5);
    public TimeSpan GetIncrementTime() => TimeSpan.FromSeconds(2);
}