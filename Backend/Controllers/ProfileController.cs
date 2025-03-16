using Backend.Models.Response;
using Backend.Repository.Interfaces;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Produces("application/json")]
[Route("api-chess/v{version:apiVersion}/[controller]")]
public class ProfileController(IUserRepository _userRepository, PotionService _potionService, PlayerDataService playerDataService): ControllerBase
{
    /// <summary>
    /// Возвращает кол-во очков у пользователя
    /// </summary>
    /// <returns></returns>
    /// <response code="200">Успешно, кол-во очков</response>
    [Authorize]
    [HttpGet("get-score")]
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
    [ProducesResponseType(typeof(ChestReward), StatusCodes.Status200OK)]
    public async Task<IActionResult> OpenChest()
    {
        var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
        var token = authHeader.Substring("Bearer ".Length);
        var dataToken = JwtService.GetJwtTokenData(token);

        var result = await playerDataService.OpenChest(dataToken.PersonId);
        
        return Ok(result);
    }
}