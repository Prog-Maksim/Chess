using Backend.Enums;

namespace Backend.Game.GameModes.Bullet;

public class Bullet4Player: IGameMode
{
    public GameMode GameMode { get; set; } = GameMode.Bullet;
    public TimeSpan GetPlayerTimeDuration() => TimeSpan.FromMinutes(1);
    public TimeSpan GetIncrementTime() => TimeSpan.FromSeconds(1);
}