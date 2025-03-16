using Backend.Enums;
using Backend.Game;
using Backend.Models.DB;
using Backend.Models.Response;
using Backend.Repository.Interfaces;
using Backend.Script;

namespace Backend.Services;

public class PlayerDataService
{
    public readonly ILeagueRepository LeagueRepository;
    private readonly IPotionRepository _potionRepository;
    private readonly IUserRepository _userRepository;

    public PlayerDataService(ILeagueRepository leagueRepository, IUserRepository userRepository, IPotionRepository potionRepository)
    {
        LeagueRepository = leagueRepository;
        _userRepository = userRepository;
        _potionRepository = potionRepository;
    }

    /// <summary>
    /// Обновляет данные полученные в результате игры
    /// </summary>
    /// <param name="score">Кол-во очков</param>
    /// <param name="rating">Кол-во рейтинга</param>
    /// <param name="player">Объект игрока</param>
    /// <param name="isWinner">Победил ли игрок</param>
    public async Task UpdatePlayerData(int score, int rating, ChessPlayer player, bool isWinner)
    {
        var playerData = await _userRepository.GetUserDataByIdAsync(player.Id);
        List<League> leagues = await LeagueRepository.GetLeagues();
        
        playerData.Score += score;
        playerData.Rating += rating;
        playerData.GamesPlayerd++;

        if (isWinner)
        {
            playerData.NumberOfWins++;
            playerData.NumberOfWinsLevel++;
        }
        
        var newLeague = leagues
            .Where(l => playerData.Rating >= l.MinRating && playerData.Rating <= l.MaxRating)
            .OrderByDescending(l => l.MinRating)
            .FirstOrDefault();
        
        if (newLeague != null && playerData.League != newLeague.LeagueName)
        {
            playerData.League = newLeague.LeagueName;
        }
        
        int requiredWins = LevelSystem.WinsRequiredForLevel(playerData.Level + 1);
        if (playerData.NumberOfWinsLevel >= requiredWins)
        {
            playerData.Level++;
            playerData.NumberOfWinsLevel = 0;
            playerData.IsChest = true;
        }
        
        await _userRepository.UpdateUserDataAsync(playerData);
    }

    public async Task<PotionEntity?> GetRandomPotion(string playerId)
    {
        var userData = await _userRepository.GetUserDataByIdAsync(playerId);
        
        Random random = new Random();
        
        // Выбираем случайное зелье из списка разблокированных
        int index = random.Next(userData.UnlockedPotions.Count);
        string potionId = userData.UnlockedPotions[index];

        // Получаем объект зелья из базы данных
        List<PotionEntity> potions = await _potionRepository.GetPotionsByIdsAsync(new List<string> { potionId });
    
        // Если зелье найдено
        if (potions.Count > 0)
        {
            PotionEntity potion = potions[0];

            // Добавляем зелье в инвентарь пользователя
            await _potionRepository.BuyPotion(playerId, potion.PotionId);
            Console.WriteLine($"Игроку {playerId} добавлено зелье: {potion.Name}");

            return potion;
        }

        return null; // Если зелье не найдено
    }

    /// <summary>
    /// Позволяет открыть сундук
    /// </summary>
    /// <param name="playerId">Идентификатор игрока</param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException">Игрок не найден</exception>
    /// <exception cref="ArgumentException">У игрока недоступен сундук для открытия</exception>
    public async Task<ChestReward> OpenChest(string playerId)
    {
        var userData = await _userRepository.GetUserDataByIdAsync(playerId);
        var player = await _userRepository.GetUserByIdAsync(playerId);
        
        if (userData == null)
            throw new NullReferenceException("Игрок не найден");
        
        if (!userData.IsChest)
            throw new ArgumentException("У игрока недоступен сундук для открытия");



        return await OpenChest(userData, player);
    }

    private async Task<ChestReward> OpenChest(UserData userData, Person player)
    {
        
        Random random = new();
        int rewardType = random.Next(100);
        ChestReward reward = new();
        
        // 70% шанс получить очки или если нет доступных зелий
        if (rewardType < 70 || userData.UnlockedPotions.Count == 0) 
        {
            int points = random.Next(10, 101);
            userData.Score += points;
            
            userData.Score = reward.Score;
            Console.WriteLine($"Игрок {player.Nickname} получил {points} очков!");
        }
        else
        {
            List<PotionEntity> allPotions = await _potionRepository.GetPotionsByIdsAsync(null);
        
            // Фильтруем зелья, которые игрок еще не разблокировал
            List<PotionEntity> unopenedPotions = allPotions
                .Where(p => !userData.UnlockedPotions.Contains(p.PotionId) && userData.Level >= p.UnlockLevel)
                .ToList();

            if (unopenedPotions.Count > 0)
            {
                int potionCount = GetPotionDropCount(random);
            
                PotionEntity newPotion = unopenedPotions[random.Next(unopenedPotions.Count)];
                
                for (int i = 0; i < potionCount; i++)
                    await _potionRepository.BuyPotion(userData.PersonId, newPotion.PotionId);
                
                
                reward.Potion = new PotionReward
                {
                    PotionId = newPotion.PotionId,
                    Name = newPotion.Name,
                    Amount = potionCount,
                    Type = newPotion.EffectType,
                    Description = newPotion.Description
                };
                
                Console.WriteLine($"Игрок {player.Nickname} получил новое зелье: {newPotion.Name}, {potionCount} шт!");
            }
            else
            {
                int points = random.Next(10, 101);
                userData.Score += points;
                
                userData.Score = reward.Score;
                Console.WriteLine($"Игрок {player.Nickname} получил {points} очков вместо зелья!");
            }
        }

        // Обновляем данные игрока в базе
        userData.IsChest = false;
        await _userRepository.UpdateUserDataAsync(userData);
        return reward;
    }
    
    
    /// <summary>
    /// Определяет количество выпадаемых зелий (чем больше — тем ниже шанс)
    /// </summary>
    private int GetPotionDropCount(Random random)
    {
        int roll = random.Next(100);
    
        if (roll < 70) return 1; // 70% шанс на 1 зелье
        if (roll < 90) return 2; // 20% шанс на 2 зелья
        return 3;                // 10% шанс на 3 зелья
    }
}