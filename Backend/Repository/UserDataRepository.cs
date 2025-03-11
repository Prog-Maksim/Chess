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

    public async Task UpdateScore(string playerId, int score)
    {
        var filter = Builders<UserData>.Filter.Eq(p => p.Id, playerId);
        var update = Builders<UserData>.Update.Set(p => p.Score, score);

        await _usersCollection.UpdateOneAsync(filter, update);
    }

    public async Task<int> GetScore(string playerId)
    {
        var filter = Builders<UserData>.Filter.Eq(p => p.Id, playerId);
        var result = await _usersCollection.Find(filter).FirstOrDefaultAsync();

        return result.Score;
    }
}