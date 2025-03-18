using Backend.Models.DB;
using Backend.Repository.Interfaces;
using MongoDB.Driver;

namespace Backend.Repository;

public class GameRepository: IGameRepository
{
    private readonly IMongoCollection<Games> _gameCollection;

    public GameRepository(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("ChessDB");
        
        _gameCollection = database.GetCollection<Games>("games");
    }

    public async Task AddGameAsync(Games game)
    {
        await _gameCollection.InsertOneAsync(game);
    }

    public async Task<List<Games>> GetGamesAsync(string playerId)
    {
        var filter = Builders<Games>.Filter.Where(g => g.ListParticipants.Contains(playerId));
        var result = await _gameCollection.Find(filter).ToListAsync();

        return result;
    }
}