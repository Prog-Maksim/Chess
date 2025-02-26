using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Backend.Models.Other;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services;

public class JwtService
{
    public static string GenerateJwtToken(string userId, string nickname)
    {
        string jti = Guid.NewGuid().ToString();
        
        var claims = new List<Claim>
        {
            new (ClaimTypes.Name, userId),
            new ("nickname", nickname),
            new (JwtRegisteredClaimNames.Jti, jti),
        };

        var jwt = new JwtSecurityToken(
            issuer: AuthOptions.Issuer,
            audience: AuthOptions.Audience,
            claims: claims,
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
        var nickName = jwtToken.Claims.First(c => c.Type == "nickname").Value;
        var jti = jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;

        return new JwtTokenData
        {
            PersonId = userId,
            Nickname = nickName,
            Jti = jti,
        };
    }
}