using System.ComponentModel.DataAnnotations;
using Backend.Enums;
using Backend.Models.DB;
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
    [AllowAnonymous]
    [HttpPost("create-potion")]
    public async Task<IActionResult> CreatePotion(PotionType potionType, string name, string description, int purchasePrice,
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
    
    
    [Authorize]
    [HttpPost("use-potion")]
    public async Task<IActionResult> UsePotion(string gameId, PotionType potionType, int? row = null, int? column = null)
    {
        var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
        var token = authHeader.Substring("Bearer ".Length);
        var dataToken = JwtService.GetJwtTokenData(token);
        
        await potionService.UsePotion(gameId, potionType, dataToken.PersonId, row, column);
        return Ok();
    }
    
    
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
    public async Task<IActionResult> UnlockPotion([Required][FromQuery] string potionId)
    {
        var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var dataToken = JwtService.GetJwtTokenData(token);

        var result = await potionService.UnlockPotion(potionId, dataToken.PersonId);
        return Ok(result);;
    }
}