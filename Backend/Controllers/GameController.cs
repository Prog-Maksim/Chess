﻿using System.ComponentModel.DataAnnotations;
using System.Net.WebSockets;
using Backend.Models.Response;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Backend.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Produces("application/json")]
[Route("api-chess/v{version:apiVersion}/[controller]")]
public class GameController(WebSocketService webSocketService, GameService gameService, ILogger<GameController> logger): ControllerBase
{
    [Authorize]
    [HttpGet("connect")]
    public async Task<IActionResult> ConnectChess()
    {
        var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
        var token = authHeader.Substring("Bearer ".Length);
        var dataToken = JwtService.GetJwtTokenData(token);
        
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using WebSocket ws = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await webSocketService.HandleConnectionAsync(ws, dataToken.PersonId);

            return Ok("Connected!");
        }
        return BadRequest();
    }
    
    [Authorize]
    [HttpPost("create-game")]
    public async Task<IActionResult> CreateGame([Required] string name, [Required] int players, [Required] bool isPrivate)
    {
        var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
        var token = authHeader.Substring("Bearer ".Length);
        var dataToken = JwtService.GetJwtTokenData(token);
        
        string gameId = gameService.CreateGame(name, players, dataToken.PersonId, dataToken.Nickname, isPrivate);
        
        return Ok(new CreateGame
        {
            Success = true,
            Message = $"Игра `{name}` успешно создана",
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
        await gameService.JoinGame(gameId, dataToken.PersonId, dataToken.Nickname, ws);
        
        return Ok(new BaseResponse
        {
            Success = true,
            Message = "Заявка на вступление в игру оставлена! ожидайте"
        });
    }

    [Authorize]
    [HttpGet("playing-field")]
    public async Task<IActionResult> GetPlayingField(string gameId)
    {
        var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
        var token = authHeader.Substring("Bearer ".Length);
        var dataToken = JwtService.GetJwtTokenData(token);
        
        var board = gameService.GetBoard(gameId, dataToken.PersonId);
        var boardStr = JsonConvert.SerializeObject(board);
        Console.WriteLine(boardStr);

        return new ContentResult
        {
            Content = boardStr,
            ContentType = "application/json",
            StatusCode = 200
        };
    }

    [Authorize]
    [HttpPost("approve-player")]
    public async Task<IActionResult> ApprovePlayerTheGame(string gameId, string personId)
    {
        var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
        var token = authHeader.Substring("Bearer ".Length);
        var dataToken = JwtService.GetJwtTokenData(token);
        
        var res = await gameService.ApprovePersonTheGame(gameId, personId, dataToken.PersonId);
        return Ok(res);
    }
    
    [Authorize]
    [HttpPost("reject-player")]
    public async Task<IActionResult> RejectPlayerTheGame(string gameId, string personId)
    {
        var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
        var token = authHeader.Substring("Bearer ".Length);
        var dataToken = JwtService.GetJwtTokenData(token);
        
        var res = await gameService.RejectPersonTheGame(gameId, personId, dataToken.PersonId);
        return Ok(res);
    }
    
    [Authorize]
    [HttpPost("leave-game")]
    public async Task<IActionResult> ExitTheGame(string gameId)
    {
        var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
        var token = authHeader.Substring("Bearer ".Length);
        var dataToken = JwtService.GetJwtTokenData(token);
        
        await gameService.LeaveTheGame(gameId, dataToken.PersonId);
        return Ok();
    }
}