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
    /// Возвращает пользователя по Id
    /// </summary>
    /// <param name="personId">Идентификатор пользователя</param>
    /// <returns></returns>
    public Task<UserData> GetUserDataByIdAsync(string personId);

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

    /// <summary>
    /// Списывает очки у пользователя
    /// </summary>
    /// <param name="playerId">Идентификатор пользователя</param>
    /// <param name="score"></param>
    /// <returns></returns>
    Task DeductScoreAsync(string playerId, int score);

    /// <summary>
    /// Разблокирует зелье у пользователя
    /// </summary>
    /// <param name="playerId">Идентификатор пользователя</param>
    /// <param name="potionId">Идентификатор зелья</param>
    /// <returns></returns>
    Task UnlockPotionAsync(string playerId, string potionId);
    
    /// <summary>
    /// Удаление аккаунта
    /// </summary>
    /// <param name="playerId">Идентификатор пользователя</param>
    /// <returns></returns>
    Task DeleteAccountAsync(string playerId);
}