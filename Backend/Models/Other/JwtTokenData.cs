namespace Backend.Models.Other;

public class JwtTokenData
{
    public required string PersonId { get; set; }
    public required string Nickname { get; set; } 
    public required string Jti { get; set; }
}