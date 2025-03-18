using System.ComponentModel.DataAnnotations;
using Backend.Enums;
using Backend.Filters;
using Backend.Models.DB;
using Backend.Models.Response;
using Backend.Repository.Interfaces;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace Backend.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Produces("application/json")]
[Route("api-chess/v{version:apiVersion}/[controller]")]
public class AdminController(IUserRepository userRepository, IConnectionMultiplexer redis): ControllerBase
{
    /// <summary>
    /// Блокирует пользователя в системе
    /// </summary>
    /// <param name="personId">Идентификатор пользователя</param>
    /// <param name="status">Статус блокировки</param>
    /// <returns></returns>
    [Authorize(Roles = "Admin")]
    [HttpPost("ban-user")]
    [ServiceFilter(typeof(ValidateJwtAccessTokenFilter))]
    public async Task<IActionResult> BanUser([Required][FromQuery] string personId, [Required][FromQuery] bool status)
    {
        var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
        var token = authHeader.Substring("Bearer ".Length);
        var database = redis.GetDatabase();

        var user = await userRepository.GetUserByIdAsync(personId);
        
        if (user == null)
        {
            var error = new BaseResponse
            {
                Success = false,
                Message = "Пользователь не найден",
                StatusCode = 404,
                Error = "NotFound"
            };

            return StatusCode(error.StatusCode, error);
        }

        if (user.Role == PersonRole.Admin)
        {
            var error = new BaseResponse
            {
                Success = false,
                Message = "Данного пользователя нельзя заблокировать",
                StatusCode = 409,
                Error = "Conflict"
            };

            return StatusCode(error.StatusCode, error);
        }
        
        await userRepository.BlockedPersonAsync(personId, status);
        TokenDb tokens = await userRepository.GetTokenDbAsync(personId);

        await JwtService.AddTokensToBan(database, personId, tokens.AccessToken);
        await JwtService.AddTokensToBan(database, personId, tokens.RefreshToken, JwtService.RefreshTokenLifetimeDay);

        string message = status ? "заблокирован": "разблокирован";
        return Ok($"Пользователь успешно {message}!");
    }
}