using Backend.Models.Other;
using StackExchange.Redis;

namespace Backend.Services;

public class AuthorizationService
{
    private readonly IConnectionMultiplexer _redis;

    public AuthorizationService(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public IDatabase GetRedisDatabase() => _redis.GetDatabase();

    public async Task<bool> ValidateAccessJwtTokenAsync(IDatabase database, JwtTokenData dataToken)
    {
        return await JwtService.ValidateAccessJwtToken(database, dataToken);
    }
}