using Backend.Enums;

namespace Backend.Game.GameModes.Classical;

public class Classical4Player: IGameMode
{
    public GameMode GameMode { get; set; } = GameMode.Classic;
    public TimeSpan GetPlayerTimeDuration() => TimeSpan.FromMinutes(60);
    public TimeSpan GetIncrementTime() => TimeSpan.FromSeconds(20);
}