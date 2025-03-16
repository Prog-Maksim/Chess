using Backend.Enums;

namespace Backend.Game.GameModes.Bullet;

public class Bullet2Player: IGameMode
{
    public GameMode GameMode { get; set; } = GameMode.Bullet;
    public TimeSpan GetPlayerTimeDuration() => TimeSpan.FromMinutes(2);
    public TimeSpan GetIncrementTime() => TimeSpan.FromSeconds(2);
}