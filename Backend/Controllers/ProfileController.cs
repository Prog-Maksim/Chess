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
public class ProfileController(IUserDataRepository _userDataRepository): ControllerBase
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

        return Ok(await _userDataRepository.GetScore(dataToken.PersonId));
    }
}