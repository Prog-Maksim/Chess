﻿using Backend.Models.DB;
using Backend.Repository.Interfaces;
using MongoDB.Driver;

namespace Backend.Repository;

public class UserRepository: IUserRepository
{
    private readonly IMongoCollection<Person> _usersCollection;
    private readonly IMongoCollection<UserData> _usersDataCollection;
    private readonly IMongoCollection<TokenDb> _usersTokenCollection;

    public UserRepository(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("ChessDB");
        
        _usersCollection = database.GetCollection<Person>("users");
        _usersDataCollection = database.GetCollection<UserData>("userData");
        _usersTokenCollection = database.GetCollection<TokenDb>("userToken");
    }
    
    public async Task<Person?> GetUserByEmailAsync(string email)
    {
        var filter = Builders<Person>.Filter.Eq(p => p.Email, email);
        var result = await _usersCollection.Find(filter).FirstOrDefaultAsync();

        return result;
    }

    public async Task<Person?> GetUserByIdAsync(string playerId)
    {
        var filter = Builders<Person>.Filter.Eq(p => p.PersonId, playerId);
        var result = await _usersCollection.Find(filter).FirstOrDefaultAsync();

        return result;
    }

    public async Task UpdatePasswordAsync(string playerId, string newPassword)
    {
        var filter = Builders<Person>.Filter.Eq(p => p.PersonId, playerId);
        var result = await _usersCollection.Find(filter).FirstOrDefaultAsync();
        
        if (result == null)
            throw new NullReferenceException("Пользователь не найден");
        
        var update = Builders<Person>.Update.Set(p => p.Password, newPassword)
            .Set(p => p.PasswordVersion, result.PasswordVersion++);

        await _usersCollection.UpdateOneAsync(filter, update);
    }

    public async Task AddUserAsync(Person user, UserData data, TokenDb tokens)
    {
        await _usersCollection.InsertOneAsync(user);
        await _usersDataCollection.InsertOneAsync(data);
        await _usersTokenCollection.InsertOneAsync(tokens);
    }
    
    public async Task<UserData> GetUserDataByIdAsync(string personId)
    {
        var filter = Builders<UserData>.Filter.Eq(p => p.PersonId, personId);
        var result = await _usersDataCollection.Find(filter).FirstOrDefaultAsync();

        if (result != null)
            return result;
        
        throw new NullReferenceException("User data not found!");
    }

    public async Task UpdateScore(string playerId, int score)
    {
        var filter = Builders<UserData>.Filter.Eq(p => p.PersonId, playerId);
        var update = Builders<UserData>.Update.Set(p => p.Score, score);

        await _usersDataCollection.UpdateOneAsync(filter, update);
    }

    public async Task<int> GetScore(string playerId)
    {
        var filter = Builders<UserData>.Filter.Eq(p => p.PersonId, playerId);
        var result = await _usersDataCollection.Find(filter).FirstOrDefaultAsync();

        return result.Score;
    }

    public async Task DeductScoreAsync(string playerId, int score)
    {
        var filter = Builders<UserData>.Filter.Eq(u => u.PersonId, playerId);
        var update = Builders<UserData>.Update.Inc(u => u.Score, -score);

        await _usersDataCollection.UpdateOneAsync(filter, update);
    }

    public async Task UnlockPotionAsync(string playerId, string potionId)
    {
        var filter = Builders<UserData>.Filter.Eq(u => u.PersonId, playerId);
        var update = Builders<UserData>.Update.AddToSet(u => u.UnlockedPotions, potionId);
        await _usersDataCollection.UpdateOneAsync(filter, update);
    }

    public async Task<TokenDb> GetTokenDbAsync(string playerId)
    {
        var filter = Builders<TokenDb>.Filter.Eq(u => u.PersonId, playerId);
        var result = await _usersTokenCollection.Find(filter).FirstOrDefaultAsync();

        return result;
    }

    public async Task UpdateToken(string playerId, TokenDb tokenDb)
    {
        var filter = Builders<TokenDb>.Filter.Eq(u => u.PersonId, playerId);
        var update = Builders<TokenDb>.Update
            .Set(u => u.AccessToken, tokenDb.AccessToken)
            .Set(u => u.RefreshToken, tokenDb.RefreshToken);

        await _usersTokenCollection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
    }

    public async Task BlockedPersonAsync(string playerId, bool status)
    {
        var filter = Builders<Person>.Filter.Eq(u => u.PersonId, playerId);
        var update = Builders<Person>.Update.Set(u => u.IsBanned, status);

        await _usersCollection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
    }

    public async Task UpdateUserDataAsync(UserData user)
    {
        var filter = Builders<UserData>.Filter.Eq(u => u.Id, user.Id);
        var update = Builders<UserData>.Update
            .Set(u => u.Score, user.Score)
            .Set(u => u.Rating, user.Rating)
            .Set(u => u.GamesPlayerd, user.GamesPlayerd)
            .Set(u => u.NumberOfWins, user.NumberOfWins)
            .Set(u => u.League, user.League)
            .Set(u => u.Level, user.Level)
            .Set(u => u.IsChest, user.IsChest)
            .Set(u => u.NumberOfWinsLevel, user.NumberOfWinsLevel);

        await _usersDataCollection.UpdateOneAsync(filter, update);
    }

    public async Task DeleteAccountAsync(string playerId)
    {
        try
        {
            var filter = Builders<Person>.Filter.Eq(p => p.PersonId, playerId);
            var filter1 = Builders<UserData>.Filter.Eq(p => p.PersonId, playerId);
            
            await _usersCollection.DeleteOneAsync(filter);
            await _usersDataCollection.DeleteOneAsync(filter1);
        }
        catch (Exception error)
        {
            Console.WriteLine(error.Message);
        }
    }
}