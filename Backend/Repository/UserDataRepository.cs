using Backend.Models.DB;
using Backend.Repository.Interfaces;
using MongoDB.Driver;

namespace Backend.Repository;

public class UserDataRepository: IUserDataRepository
{
    private readonly IMongoCollection<UserData> _usersCollection;

    public UserDataRepository(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("ChessDB");
        _usersCollection = database.GetCollection<UserData>("userData");
    }
    
    public async Task AddUserDataAsync(UserData data)
    {
        await _usersCollection.InsertOneAsync(data);
    }

    public async Task<UserData> GetUserDataByIdAsync(string personId)
    {
        var filter = Builders<UserData>.Filter.Eq(p => p.PersonId, personId);
        var result = await _usersCollection.Find(filter).FirstOrDefaultAsync();

        if (result != null)
            return result;
        
        throw new NullReferenceException("User data not found!");
    }

    public async Task UpdateScore(string playerId, int score)
    {
        var filter = Builders<UserData>.Filter.Eq(p => p.PersonId, playerId);
        var update = Builders<UserData>.Update.Set(p => p.Score, score);

        await _usersCollection.UpdateOneAsync(filter, update);
    }

    public async Task<int> GetScore(string playerId)
    {
        var filter = Builders<UserData>.Filter.Eq(p => p.PersonId, playerId);
        var result = await _usersCollection.Find(filter).FirstOrDefaultAsync();

        return result.Score;
    }

    public async Task DeductScoreAsync(string playerId, int score)
    {
        var filter = Builders<UserData>.Filter.Eq(u => u.PersonId, playerId);
        var update = Builders<UserData>.Update.Inc(u => u.Score, -score);

        await _usersCollection.UpdateOneAsync(filter, update);
    }

    public async Task UnlockPotionAsync(string playerId, string potionId)
    {
        var filter = Builders<UserData>.Filter.Eq(u => u.PersonId, playerId);
        var update = Builders<UserData>.Update.AddToSet(u => u.UnlockedPotions, potionId);
        await _usersCollection.UpdateOneAsync(filter, update);
    }

    public async Task DeleteAccountAsync(string playerId)
    {
        var filter = Builders<UserData>.Filter.Eq(p => p.PersonId, playerId);
        await _usersCollection.DeleteOneAsync(filter);
    }
}