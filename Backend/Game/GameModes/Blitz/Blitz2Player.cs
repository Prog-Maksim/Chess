namespace Backend.Game.GameModes.Blitz;

public class Blitz2Player: IGameMode
{
    public TimeSpan GetPlayerTimeDuration() => TimeSpan.FromMinutes(8);
    public TimeSpan GetIncrementTime() => TimeSpan.FromSeconds(3);
}