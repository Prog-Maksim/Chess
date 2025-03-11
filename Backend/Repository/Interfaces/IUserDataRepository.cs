using Backend.Models.DB;

namespace Backend.Repository.Interfaces;

public interface IUserDataRepository
{
    /// <summary>
    /// Добавляет данные пользователя
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public Task AddUserDataAsync(UserData data);

    /// <summary>
    /// Обновляет очки у пользователя
    /// </summary>
    /// <param name="score"></param>
    /// <returns></returns>
    public Task UpdateScore(string playerId, int score);
}