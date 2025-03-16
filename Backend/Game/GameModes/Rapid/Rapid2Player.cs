using Backend.Enums;

namespace Backend.Game.GameModes.Rapid;

public class Rapid2Player: IGameMode
{
    public GameMode GameMode { get; set; } = GameMode.Rapid;
    public TimeSpan GetPlayerTimeDuration() => TimeSpan.FromMinutes(40);
    public TimeSpan GetIncrementTime() => TimeSpan.FromSeconds(10);
}