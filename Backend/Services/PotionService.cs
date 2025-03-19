using Backend.Enums;
using Backend.Game;
using Backend.Game.Potion;
using Backend.Game.Shapes;
using Backend.Models.DB;
using Backend.Models.Response;
using Backend.Repository.Interfaces;
using Backend.Script;
using MongoDB.Driver;

namespace Backend.Services;

public class PotionService
{
    private readonly IPotionRepository _potionRepository;
    private readonly GameService _gameService;
    private readonly IUserRepository _userRepository;

    public PotionService(IPotionRepository potionRepository, GameService gameService, IUserRepository userRepository)
    {
        _potionRepository = potionRepository;
        _gameService = gameService;
        _userRepository = userRepository;
    }

    public async Task UsePotion(string gameId, PotionType potionType, string playerId, int? row = null, int? column = null)
    {
        BaseChessGame game = _gameService.GetChessGame(gameId);

        ChessPiece? piece = null;
        if (row != null || column != null)
            piece = _gameService.GetChessPiece(game, row?? 0, column?? 0);
        
        ChessPlayer player = _gameService.GetChessPlayer(game, playerId);
        
        var data = await _potionRepository.GetPotionDataByUserIdAsync(playerId);
        var potionData = await _potionRepository.GetPotionsByTypeAsync(potionType);
        
        if (!player.AvailablePotion.Contains(potionType))
            throw new InvalidOperationException("Данное зелье недоступно в данной игре");
        
        if (data.FirstOrDefault(d => d.PotionId == potionData.PotionId) == null)
            throw new InvalidOperationException("У вас недостаточно данного зелья");
        
        PotionEntity entity = await _potionRepository.GetPotionAsync(potionType);
        IPotion potion = PotionFactory.CreatePotion(entity);

        await potion.ApplyEffect(game, player, piece);
        await _potionRepository.DeductPotionCountAsync(playerId, potionData.PotionId);
    }

    public async Task<PersonData> GetPersonData(string personId)
    {
        var personTask = _userRepository.GetUserByIdAsync(personId);
        var userDataTask = _userRepository.GetUserDataByIdAsync(personId);
        var dataPotionTask = _potionRepository.GetPotionDataByUserIdAsync(personId);
        var allPotionsTask = _potionRepository.GetAllPotionsAsync();
        
        await Task.WhenAll(personTask, userDataTask, dataPotionTask, allPotionsTask);

        var person = personTask.Result;
        var userData = userDataTask.Result;
        var dataPotion = dataPotionTask.Result;
        var allPotions = allPotionsTask.Result;
        
        var data = new PersonData
        {
            PersonId = person.PersonId,
            Name = person.Nickname,
            League = userData.League,
            Level = userData.Level,
            Score = userData.Score,
            IsChest = userData.IsChest,
            NumberOfWinsLevel = userData.NumberOfWinsLevel,
            RequiredNumberOfWinsLevel = LevelSystem.WinsRequiredForLevel(userData.Level + 1),
            Potions = allPotions.Select(potion =>
            {
                var potionUserData = dataPotion.FirstOrDefault(x => x.PotionId == potion.PotionId);
                return new PotionData
                {
                    PotionId = potion.PotionId,
                    Type = potion.EffectType,
                    Count = potionUserData?.Quantity ?? 0,
                    IsPurchased = userData.UnlockedPotions.Contains(potion.PotionId),
                    IsUnlocked = userData.Level >= potion.UnlockLevel
                };
            }).ToList()
        };

        return data;
    }

    public async Task<bool> BuyPotion(string potionId, string personId)
    {
        var person = await _userRepository.GetUserByIdAsync(personId);
        var userData = await _userRepository.GetUserDataByIdAsync(personId);
        var potion = await _potionRepository.GetPotionByIdAsync(potionId);
        
        // Проверяем, разблокировано ли зелье
        Console.WriteLine("Проверка разблокировки зелья");
        bool isUnlocked = userData.Level >= potion.UnlockLevel;
        if (!isUnlocked)
            return false;
        
        // Проверяем, есть ли у пользователя нужное количество звёзд
        Console.WriteLine("Проверяем баланс звезд");
        int potionCost = potion.PurchasePrice;
        if (userData.Score < potionCost)
            return false;
        
        // Проверяем, покупал ли пользователь это зелье ранее
        Console.WriteLine("Проверка приобрел ли пользователь зелье");
        bool isPurchased = userData.UnlockedPotions?.Contains(potionId) ?? false;
        if (!isPurchased)
            return false;
        
        await _userRepository.DeductScoreAsync(personId, potionCost);
        await _potionRepository.BuyPotion(personId, potionId);

        return true;
    }

    public async Task<bool> UnlockPotion(string potionId, string personId)
    {
        var person = await _userRepository.GetUserByIdAsync(personId);
        var userData = await _userRepository.GetUserDataByIdAsync(personId);
        var potion = await _potionRepository.GetPotionByIdAsync(potionId);
        
        // Проверяем, разблокировано ли зелье
        Console.WriteLine("Проверка разблокировки зелья");
        bool isUnlocked = userData.Level >= potion.UnlockLevel;
        if (!isUnlocked)
            return false;
        
        // Проверяем, покупал ли пользователь это зелье ранее
        Console.WriteLine("Проверка приобрел ли пользователь зелье");
        bool isPurchased = userData.UnlockedPotions?.Contains(potionId) ?? false;
        if (isPurchased)
            return false; // зелье уже разблокировано
        
        // Проверяем, есть ли у пользователя нужное количество звёзд
        Console.WriteLine("Проверяем баланс звезд");
        int potionCost = potion.UnlockPrice;
        if (userData.Score < potionCost)
            return false;
        
        await _userRepository.UnlockPotionAsync(personId, potionId);
        await _userRepository.DeductScoreAsync(personId, potionCost);

        return true;
    }
}