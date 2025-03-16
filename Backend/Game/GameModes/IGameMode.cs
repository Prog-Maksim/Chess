using Backend.Enums;

namespace Backend.Game.GameModes;

public interface IGameMode
{
    public GameMode GameMode { get; set; }
    public TimeSpan GetPlayerTimeDuration();
    public TimeSpan GetIncrementTime();
}