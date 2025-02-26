using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services;

public class JwtService
{
    public static string GenerateJwtToken(string userId)
    {
        string jti = Guid.NewGuid().ToString();
        
        var claims = new List<Claim>
        {
            new (ClaimTypes.Name, userId),
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
}