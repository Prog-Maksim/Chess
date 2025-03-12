namespace Backend.Game.GameModes;

public interface IGameMode
{
    public TimeSpan GetPlayerTimeDuration();
    public TimeSpan GetIncrementTime();
}