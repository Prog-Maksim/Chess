using System.ComponentModel.DataAnnotations;
using Backend.Enums;
using Backend.Filters;
using Backend.Models.DB;
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
public class PotionController(PotionService potionService, IPotionRepository potionRepository): ControllerBase
{
    /// <summary>
    /// Создание информации о зелье
    /// </summary>
    /// <param name="potionType"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="purchasePrice"></param>
    /// <param name="unlockPrice"></param>
    /// <param name="unlockLevel"></param>
    /// <returns></returns>
    [Authorize(Roles = "Admin")]
    [HttpPost("create-potion")]
    public async Task<IActionResult> CreatePotion([Required][FromQuery] PotionType potionType, [Required][FromQuery] string name, [Required][FromQuery] string description, [Required][FromQuery] int purchasePrice,
        int unlockPrice, int unlockLevel)
    {
        PotionEntity entity = new()
        {
            PotionId = Guid.NewGuid().ToString(),
            Name = name,
            Description = description,
            PurchasePrice = purchasePrice,
            UnlockPrice = unlockPrice,
            EffectType = potionType,
            UnlockLevel = unlockLevel
        };

        await potionRepository.AddPotion(entity);
        return Ok();
    }
    
    
    /// <summary>
    /// Использование зелья
    /// </summary>
    /// <param name="gameId">Идентификатор игры</param>
    /// <param name="potionType">Тип используемого зелья</param>
    /// <param name="row">Столбец применения зелья</param>
    /// <param name="column">Строка применения зелья</param>
    /// <returns></returns>
    /// <response code="200">Успешно</response>
    /// <response code="400">Некорректный вызов зелья</response>
    /// <response code="401">Не авторизован</response>
    /// <response code="409">Запрещено использование зелья</response>
    [Authorize]
    [HttpPost("use-potion")]
    [ServiceFilter(typeof(ValidateJwtAccessTokenFilter))]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UsePotion([Required][FromQuery] string gameId, [Required][FromQuery] PotionType potionType, [FromQuery] int? row = null, [FromQuery] int? column = null)
    {
        var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
        var token = authHeader.Substring("Bearer ".Length);
        var dataToken = JwtService.GetJwtTokenData(token);

        try
        {
            await potionService.UsePotion(gameId, potionType, dataToken.PersonId, row, column);
            return Ok(true);
        }
        catch (InvalidOperationException error)
        {
            return StatusCode(409, new BaseResponse
            {
                Message = error.Message,
                StatusCode = 409,
                Success = false,
                Error = "Conflict"
            });
        }
        catch (UnauthorizedAccessException error)
        {
            return StatusCode(409, new BaseResponse
            {
                Message = error.Message,
                StatusCode = 409,
                Success = false,
                Error = "Conflict"
            });
        }
        catch (ArgumentNullException error)
        {
            return StatusCode(400, new BaseResponse
            {
                Message = error.Message,
                StatusCode = 400,
                Success = false,
                Error = "Bad Request"
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    /// <summary>
    /// Выдача информации о зельях
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet("get-data-potion")]
    public async Task<IActionResult> GetPotionsData()
    {
        List<PotionEntity> potions = new List<PotionEntity>();
        
        foreach (PotionType type in Enum.GetValues(typeof(PotionType)))
        {
            PotionEntity potion = await potionRepository.GetPotionAsync(type);
            potions.Add(potion);
        }

        potions = potions.OrderBy(p => p.UnlockLevel).ToList();
        return Ok(potions);
    }

    /// <summary>
    /// Позволяет докупить зелье 
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpPost("buy-potion")]
    [ServiceFilter(typeof(ValidateJwtAccessTokenFilter))]
    public async Task<IActionResult> BuyPotion([Required][FromQuery] string potionId)
    {
        var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var dataToken = JwtService.GetJwtTokenData(token);
        
        var result = await potionService.BuyPotion(potionId, dataToken.PersonId);
        return Ok(result);
    }
    
    /// <summary>
    /// Позволяет разблокировать зелье
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpPost("unlock-potion")]
    [ServiceFilter(typeof(ValidateJwtAccessTokenFilter))]
    public async Task<IActionResult> UnlockPotion([Required][FromQuery] string potionId)
    {
        var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var dataToken = JwtService.GetJwtTokenData(token);

        var result = await potionService.UnlockPotion(potionId, dataToken.PersonId);
        return Ok(result);;
    }
}