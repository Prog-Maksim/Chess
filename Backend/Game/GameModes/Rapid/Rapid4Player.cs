namespace Backend.Game.GameModes.Rapid;

public class Rapid4Player: IGameMode
{
    public TimeSpan GetPlayerTimeDuration() => TimeSpan.FromMinutes(30);
    public TimeSpan GetIncrementTime() => TimeSpan.FromSeconds(5);
}