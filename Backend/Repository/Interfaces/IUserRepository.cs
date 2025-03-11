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
    Task AddUserAsync(Person user);
    
    /// <summary>
    /// Сохраняет изменентя в БД
    /// </summary>
    /// <returns></returns>
    Task SaveChangesAsync();
}