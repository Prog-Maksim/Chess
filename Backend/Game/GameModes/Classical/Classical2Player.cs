namespace Backend.Game.GameModes.Classical;

public class Classical2Player: IGameMode
{
    public TimeSpan GetPlayerTimeDuration() => TimeSpan.FromMinutes(90);
    public TimeSpan GetIncrementTime() => TimeSpan.FromSeconds(30);
}