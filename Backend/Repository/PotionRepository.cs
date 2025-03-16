using Backend.Enums;
using Backend.Models.DB;
using Backend.Repository.Interfaces;
using MongoDB.Driver;

namespace Backend.Repository;

public class PotionRepository: IPotionRepository
{
    private readonly IMongoCollection<PotionEntity> _potionCollection;
    private readonly IMongoCollection<UserPotionInventory> _potionInventoryCollection;

    public PotionRepository(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("ChessDB");
        
        _potionCollection = database.GetCollection<PotionEntity>("potions");
        _potionInventoryCollection = database.GetCollection<UserPotionInventory>("potionsInventories");
    }


    public async Task AddPotion(PotionEntity entity)
    {
        await _potionCollection.InsertOneAsync(entity);
    }

    public async Task<PotionEntity> GetPotionAsync(PotionType potion)
    {
        var filter = Builders<PotionEntity>.Filter.Eq(p => p.EffectType, potion);
        var result = await _potionCollection.Find(filter).FirstOrDefaultAsync();

        if (result == null)
            throw new NullReferenceException("Potion not found");
        
        return result;
    }

    public async Task<PotionEntity> GetPotionByIdAsync(string potionId)
    {
        var filter = Builders<PotionEntity>.Filter.Eq(p => p.PotionId, potionId);
        var result = await _potionCollection.Find(filter).FirstOrDefaultAsync();

        if (result == null)
            throw new NullReferenceException("Potion not found");
        
        return result;
    }

    public async Task<List<PotionEntity>> GetPotionsByIdsAsync(List<string>? potionIds)
    {
        if (potionIds == null || !potionIds.Any())
            return new List<PotionEntity>();

        var filter = Builders<PotionEntity>.Filter.In(p => p.PotionId, potionIds);
        return await _potionCollection.Find(filter).ToListAsync();
    }

    public async Task<PotionEntity> GetPotionsByTypeAsync(PotionType type)
    {
        var filter = Builders<PotionEntity>.Filter.Eq(p => p.EffectType, type);
        var result = await _potionCollection.Find(filter).FirstOrDefaultAsync();

        if (result == null)
            throw new NullReferenceException("Potion not found");
        
        return result;
    }
    
    public async Task DeductPotionCountAsync(string playerId, string potionId)
    {
        var filter = Builders<UserPotionInventory>.Filter.And(
            Builders<UserPotionInventory>.Filter.Eq(u => u.PotionId, potionId),
            Builders<UserPotionInventory>.Filter.Eq(u => u.PersonId, playerId)
        );
        
        var update = Builders<UserPotionInventory>.Update.Inc(u => u.Quantity, -1);

        await _potionInventoryCollection.UpdateOneAsync(filter, update);
    }

    public async Task<List<PotionEntity>> GetAllPotionsAsync()
    {
        return await _potionCollection.Find(FilterDefinition<PotionEntity>.Empty).ToListAsync();
    }

    public async Task<List<UserPotionInventory>> GetPotionDataByUserIdAsync(string personId)
    {
        var filter = Builders<UserPotionInventory>.Filter.Eq(p => p.PersonId, personId);
        var result = await _potionInventoryCollection.Find(filter).ToListAsync();
        
        if (result == null)
            throw new NullReferenceException("Potion data not found");
        
        return result;
    }

    public async Task BuyPotion(string personId, string potionId)
    {
        var filter = Builders<UserPotionInventory>.Filter.And(
            Builders<UserPotionInventory>.Filter.Eq(p => p.PersonId, personId),
            Builders<UserPotionInventory>.Filter.Eq(p => p.PotionId, potionId)
        );

        var existingPotion = await _potionInventoryCollection.Find(filter).FirstOrDefaultAsync();

        if (existingPotion == null)
        {
            var newPotion = new UserPotionInventory
            {
                PotionId = potionId,
                PersonId = personId,
                Quantity = 1
            };
            await _potionInventoryCollection.InsertOneAsync(newPotion);
        }
        else
        {
            var update = Builders<UserPotionInventory>.Update.Inc(p => p.Quantity, 1);
            await _potionInventoryCollection.UpdateOneAsync(filter, update);
        }
    }
}