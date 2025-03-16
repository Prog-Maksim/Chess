using Backend.Models.DB;
using Backend.Repository.Interfaces;
using MongoDB.Driver;

namespace Backend.Repository;

public class LeagueRepository: ILeagueRepository
{
    private readonly IMongoCollection<League> _leagueCollection;

    public LeagueRepository(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("ChessDB");
        
        _leagueCollection = database.GetCollection<League>("leagues");
        Task.Run(() => EnsureSeedDataAsync()).Wait();
    }
    
    private async Task EnsureSeedDataAsync()
    {
        var count = await _leagueCollection.CountDocumentsAsync(FilterDefinition<League>.Empty);

        if (count == 0)
        {
            var initialLeagues = new List<League>
            {
                new() { LeagueId = 1, RatingIsWinner = 15, LeagueName = "Пешечник", MinRating = 0, MaxRating = 800 },
                new() { LeagueId = 2, RatingIsWinner = 12, LeagueName = "Тактик", MinRating = 800, MaxRating = 1200 },
                new() { LeagueId = 3, RatingIsWinner = 10, LeagueName = "Стратег", MinRating = 1200, MaxRating = 1600 },
                new() { LeagueId = 4, RatingIsWinner = 8, LeagueName = "Магистр доски", MinRating = 1600, MaxRating = 2000 },
                new() { LeagueId = 5, RatingIsWinner = 5, LeagueName = "Гроссмейстер", MinRating = 2000, MaxRating = 2500 },
                new() { LeagueId = 6, RatingIsWinner = 3, LeagueName = "Властелин шахмат", MinRating = 2500, MaxRating = 100_000 },
            };
            await _leagueCollection.InsertManyAsync(initialLeagues);
        }
    }

    public async Task<List<League>> GetLeagues()
    {
        return await _leagueCollection.Find(FilterDefinition<League>.Empty).ToListAsync();
    }
}