namespace Backend.Game.GameModes.Classical;

public class Classical4Player: IGameMode
{
    public TimeSpan GetPlayerTimeDuration() => TimeSpan.FromMinutes(60);
    public TimeSpan GetIncrementTime() => TimeSpan.FromSeconds(20);
}