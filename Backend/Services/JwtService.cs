using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Backend.Enums;
using Backend.Models.DB;
using Backend.Models.Other;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace Backend.Services;

public class JwtService
{
    public const int AccessTokenLifetimeDay = 30;
    public const int RefreshTokenLifetimeDay = 365;
    
    public static string GenerateJwtAccessToken(string userId, string nickname, PersonRole role)
    {
        string jti = Guid.NewGuid().ToString();
        
        var claims = new List<Claim>
        {
            new (ClaimTypes.Name, userId),
            new ("nickname", nickname),
            new ("token_type", TokenType.Access.ToString()),
            new (ClaimTypes.Role, role.ToString()),
            new (JwtRegisteredClaimNames.Jti, jti),
        };

        var jwt = new JwtSecurityToken(
            issuer: AuthOptions.Issuer,
            audience: AuthOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(AccessTokenLifetimeDay),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
    
    public static string GenerateJwtRefreshToken(string userId, int passwordVersion, PersonRole role)
    {
        string jti = Guid.NewGuid().ToString();
        
        var claims = new List<Claim>
        {
            new (ClaimTypes.Name, userId),
            new ("token_type", TokenType.Refresh.ToString()),
            new (ClaimTypes.Role, role.ToString()),
            new ("version", passwordVersion.ToString()),
            new (JwtRegisteredClaimNames.Jti, jti),
        };

        var jwt = new JwtSecurityToken(
            issuer: AuthOptions.Issuer,
            audience: AuthOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(RefreshTokenLifetimeDay),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    public static JwtTokenData GetJwtTokenData(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        
        if (!handler.CanReadToken(token))
            throw new ArgumentException("Неверный jwt токен");
        
        var jwtToken = handler.ReadJwtToken(token);
        
        var userId = jwtToken.Claims.First(c => c.Type == ClaimTypes.Name).Value;
        var nickName = jwtToken.Claims.FirstOrDefault(c => c.Type == "nickname")?.Value;
        var jti = jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
        var tokenType = jwtToken.Claims.First(c => c.Type == "token_type").Value;
        var tokenTypeEnum = Enum.Parse<TokenType>(tokenType);
        var versionClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "version")?.Value;
        var roles = Enum.Parse<PersonRole>(jwtToken.Claims.First(c => c.Type == ClaimTypes.Role).Value);
        
        int version = 0;
        if (int.TryParse(versionClaim, out var parsedVersion))
            version = parsedVersion;

        return new JwtTokenData
        {
            PersonId = userId,
            Nickname = nickName,
            Jti = jti,
            TokenType = tokenTypeEnum,
            Role = roles,
            Token = token,
            Version = version
        };
    }
    
    public static bool ValidateRefreshJwtToken(JwtTokenData token, Person user)
    {
        if (token.TokenType != TokenType.Refresh)
            return false;

        if (token.Version != user.PasswordVersion)
            return false;
        
        return true;
    }
    
    public static async Task<bool> ValidateAccessJwtToken(IDatabase database, JwtTokenData token)
    {
        if (token.TokenType != TokenType.Access)
            return true;
        
        return await IsTokenBannedAsync(database, token.PersonId, token.Token);
    }
    
    public static async Task AddTokensToBan(IDatabase database, string userId, string token, int expiresDay = AccessTokenLifetimeDay)
    {
        var tag = $"ban:{userId}";
        
        await database.SetAddAsync(tag, token);
        await database.KeyExpireAsync(tag, TimeSpan.FromDays(expiresDay));
    }

    private static async Task<bool> IsTokenBannedAsync(IDatabase database, string userId, string token)
    {
        var tag = $"ban:{userId}";
        bool isBanned = await database.SetContainsAsync(tag, token);

        return isBanned;
    }
}