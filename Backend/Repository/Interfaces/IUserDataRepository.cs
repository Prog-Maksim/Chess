using Backend.Models.DB;

namespace Backend.Repository.Interfaces;

public interface IUserDataRepository
{
    /// <summary>
    /// Добавляет данные пользователя
    /// </summary>
    /// <param name="data">Данные пользователя</param>
    /// <returns></returns>
    public Task AddUserDataAsync(UserData data);

    /// <summary>
    /// Обновляет очки у пользователя
    /// </summary>
    /// <param name="playerId">Идентификатор пользователя</param>
    /// <param name="score">Кол-во заработанных очков</param>
    /// <returns></returns>
    public Task UpdateScore(string playerId, int score);
    
    /// <summary>
    /// Возвращает кол-во очков у пользователя
    /// </summary>
    /// <param name="playerId">Идентификатор пользователя</param>
    /// <returns></returns>
    public Task<int> GetScore(string playerId);
}