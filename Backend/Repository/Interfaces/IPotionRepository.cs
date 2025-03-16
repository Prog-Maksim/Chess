using Backend.Enums;
using Backend.Models.DB;

namespace Backend.Repository.Interfaces;

public interface IPotionRepository
{
    /// <summary>
    /// Добавляет зелье в БД
    /// </summary>
    /// <param name="entity">Объект зелья</param>
    /// <returns></returns>
    public Task AddPotion(PotionEntity entity);
    
    /// <summary>
    /// Возвращает данные зелья
    /// </summary>
    /// <param name="potion">Тип зелья</param>
    /// <returns></returns>
    public Task<PotionEntity> GetPotionAsync(PotionType potion);
    
    /// <summary>
    /// Возвращает данные зелья по d
    /// </summary>
    /// <param name="potionId">Идентификатор зелья</param>
    /// <returns></returns>
    public Task<PotionEntity> GetPotionByIdAsync(string potionId);
    
    /// <summary>
    /// Списывает зелье у пользователя
    /// </summary>
    /// <param name="playerId">Идентификатор пользователя</param>
    /// <param name="potionId">Идентификатор зелья</param>
    /// <returns></returns>
    Task DeductPotionCountAsync(string playerId, string potionId);
    
    /// <summary>
    /// Возвращает данные зелья по id
    /// </summary>
    /// <param name="potionIds">Идентификатор зелья</param>
    /// <returns></returns>
    public Task<List<PotionEntity>> GetPotionsByIdsAsync(List<string>? potionIds);

    /// <summary>
    /// Возвращает данные зелья по типу
    /// </summary>
    /// <param name="type">Тип зелья</param>
    /// <returns></returns>
    public Task<PotionEntity> GetPotionsByTypeAsync(PotionType type);
    
    /// <summary>
    /// Возвращает все данные зелья
    /// </summary>
    /// <returns></returns>
    public Task<List<PotionEntity>> GetAllPotionsAsync();

    /// <summary>
    /// Возвращает данные купленных зелий
    /// </summary>
    /// <param name="personId">Идентификатор пользователя</param>
    /// <returns></returns>
    public Task<List<UserPotionInventory>> GetPotionDataByUserIdAsync(string personId);

    /// <summary>
    /// Покупка зелья
    /// </summary>
    /// <param name="personId">Идентификатор пользователя</param>
    /// <param name="potionId">Идентификатор зелья</param>
    /// <returns></returns>
    public Task BuyPotion(string personId, string potionId);
}