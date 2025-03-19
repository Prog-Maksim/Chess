using Backend.Filters;
using Backend.Models.Response;
using Backend.Repository;
using Backend.Repository.Interfaces;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Produces("application/json")]
[Route("api-chess/v{version:apiVersion}/[controller]")]
public class ProfileController(IUserRepository _userRepository, PotionService _potionService, PlayerDataService playerDataService, IGameRepository gameRepository): ControllerBase
{
    /// <summary>
    /// Возвращает кол-во очков у пользователя
    /// </summary>
    /// <returns></returns>
    /// <response code="200">Успешно, кол-во очков</response>
    [Authorize]
    [HttpGet("get-score")]
    [ServiceFilter(typeof(ValidateJwtAccessTokenFilter))]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetScore()
    {
        var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
        var token = authHeader.Substring("Bearer ".Length);
        var dataToken = JwtService.GetJwtTokenData(token);

        return Ok(await _userRepository.GetScore(dataToken.PersonId));
    }
    
    /// <summary>
    /// Возвращает данные пользователя
    /// </summary>
    /// <returns></returns>
    /// <response code="200">Успешно</response>
    [Authorize]
    [HttpGet("get-player-data")]
    [ServiceFilter(typeof(ValidateJwtAccessTokenFilter))]
    [ProducesResponseType(typeof(PersonData), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlayerData()
    {
        var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var dataToken = JwtService.GetJwtTokenData(token);
        
        var data = await _potionService.GetPersonData(dataToken.PersonId);
        return Ok(data);
    }
    
    /// <summary>
    /// Позволяет удалить аккаунт пользователя
    /// </summary>
    /// <returns></returns>
    /// <response code="200">Успешно</response>
    [Authorize]
    [HttpDelete("account")]
    [ServiceFilter(typeof(ValidateJwtAccessTokenFilter))]
    // [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteAccount()
    {
        var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
        var token = authHeader.Substring("Bearer ".Length);
        var dataToken = JwtService.GetJwtTokenData(token);
        
        await _userRepository.DeleteAccountAsync(dataToken.PersonId);
        
        return Ok(new BaseResponse
        {
            Message = "Аккаунт успешно удален",
            Success = true,
            StatusCode = 200
        });
    }


    /// <summary>
    /// Позволяет игроку открыть сундук
    /// </summary>
    /// <returns></returns>
    /// <response code="200">Успешно</response>
    [Authorize]
    [HttpPost("chest")]
    [ServiceFilter(typeof(ValidateJwtAccessTokenFilter))]
    [ProducesResponseType(typeof(ChestReward), StatusCodes.Status200OK)]
    public async Task<IActionResult> OpenChest()
    {
        var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
        var token = authHeader.Substring("Bearer ".Length);
        var dataToken = JwtService.GetJwtTokenData(token);

        var result = await playerDataService.OpenChest(dataToken.PersonId);
        return Ok(result);
    }

    /// <summary>
    /// Возвращает игроку все игры
    /// </summary>
    /// <returns></returns>
    /// <response code="200">Успешно</response>
    [Authorize]
    [HttpGet("games")]
    [ServiceFilter(typeof(ValidateJwtAccessTokenFilter))]
    [ProducesResponseType(typeof(List<GamesHistory>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGames()
    {
        var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
        var token = authHeader.Substring("Bearer ".Length);
        var dataToken = JwtService.GetJwtTokenData(token);

        List<GamesHistory> games = new List<GamesHistory>();
        var gamesData = await gameRepository.GetGamesAsync(dataToken.PersonId);

        foreach (var game in gamesData)
        {
            GamesHistory gameHistory = new GamesHistory
            {
                GameName = game.Title,
                GameDuration = game.DurationGame,
                IsWinner = game.WinePersonId == dataToken.PersonId,
                PlayerCount = game.ListParticipants.Count,
                GameMode = game.GameMode,
                IsPrivate = game.IsPrivate,
                IsPotion = game.IsPotion,
                DateCreated = game.DateCreated,
            };
            games.Add(gameHistory);
        }
        
        return Ok(games);
    }
}