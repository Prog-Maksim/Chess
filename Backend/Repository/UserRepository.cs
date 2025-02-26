using Backend.Models.DB;
using Backend.Repository.Interfaces;
using MongoDB.Driver;

namespace Backend.Repository;

public class UserRepository: IUserRepository
{
    private readonly IMongoCollection<Person> _usersCollection;

    public UserRepository(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("ChessDB");
        _usersCollection = database.GetCollection<Person>("users");
    }
    
    public async Task<Person?> GetUserByEmailAsync(string email)
    {
        var filter = Builders<Person>.Filter.Eq(p => p.Email, email);
        var result = await _usersCollection.Find(filter).FirstOrDefaultAsync();

        return result;
    }

    public async Task AddUserAsync(Person user)
    {
        await _usersCollection.InsertOneAsync(user);
    }

    public async Task SaveChangesAsync()
    {
    }
}