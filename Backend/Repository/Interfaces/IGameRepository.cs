using Backend.Models.DB;

namespace Backend.Repository.Interfaces;

public interface IGameRepository
{
    /// <summary>
    /// Добавляет игру в БД
    /// </summary>
    /// <param name="game">Данные об игре</param>
    /// <returns></returns>
    public Task AddGameAsync(Games game);

    /// <summary>
    /// Возвращает все игры с данным пользователем
    /// </summary>
    /// <param name="playerId">Идентификатор игры</param>
    /// <returns></returns>
    public Task<List<Games>> GetGamesAsync(string playerId);
}