using System.Net.WebSockets;
using Backend.Models.Response;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Produces("application/json")]
[Route("api/v{version:apiVersion}/[controller]")]
public class GameController(WebSocketService webSocketService, GameService gameService): ControllerBase
{
    [Authorize]
    [HttpGet("connect")]
    public async Task<IActionResult> ConnectChat()
    {
        var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
        var token = authHeader.Substring("Bearer ".Length);
        var dataToken = JwtService.GetJwtTokenData(token);
        
        await webSocketService.HandleConnectionAsync(HttpContext, dataToken.PersonId);
        return Ok("Connected!");
    }
    
    [Authorize]
    [HttpPost("create-game")]
    public async Task<IActionResult> CreateGame()
    {
        var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
        var token = authHeader.Substring("Bearer ".Length);
        var dataToken = JwtService.GetJwtTokenData(token);
        
        string gameId = gameService.CreateGame2Players(dataToken.PersonId, dataToken.Nickname);
        
        return Ok(new CreateGame
        {
            Success = true,
            Message = "Игра 2 на 2 успешно создана",
            GameId = gameId
        });
    }
    
    [Authorize]
    [HttpPost("login-game")]
    public async Task<IActionResult> LogInGame(string gameId)
    {
        var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
        var token = authHeader.Substring("Bearer ".Length);
        var dataToken = JwtService.GetJwtTokenData(token);

        WebSocket ws = webSocketService.GetWebSocket(dataToken.PersonId);
        gameService.JoinGame(gameId, dataToken.PersonId, dataToken.Nickname, ws);
        
        return Ok(new BaseResponse
        {
            Success = true,
            Message = "Заявка на вступление в игру оставлена! ожидайте"
        });
    }
}