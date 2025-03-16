using Backend.Models.DB;

namespace Backend.Repository.Interfaces;

public interface IUserRepository
{
    /// <summary>
    /// Возвращает пользователя по почте 
    /// </summary>
    /// <param name="email">Почта пользователя</param>
    /// <returns></returns>
    Task<Person?> GetUserByEmailAsync(string email);

    /// <summary>
    /// Возвращает пользователя по id 
    /// </summary>
    /// <param name="playerId">Идентификатор  пользователя</param>
    /// <returns></returns>
    Task<Person?> GetUserByIdAsync(string playerId);

    /// <summary>
    /// Обновляет пароль пользователя 
    /// </summary>
    /// <param name="playerId">Идентификатор  пользователя</param>
    /// <param name="newPassword">Новый пароль</param>
    /// <returns></returns>
    Task UpdatePasswordAsync(string playerId, string newPassword);
    
    /// <summary>
    /// Добавляет пользователя в БД
    /// </summary>
    /// <param name="user">Пользователь</param>
    /// <returns></returns>
    Task AddUserAsync(Person user, UserData data);
    
    /// <summary>
    /// Удаление аккаунта
    /// </summary>
    /// <param name="playerId">Идентификатор пользователя</param>
    /// <returns></returns>
    Task DeleteAccountAsync(string playerId);
    
    
    
    
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
    /// Обновляет данные пользователя в БД
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public Task UpdateUserDataAsync(UserData user);
}