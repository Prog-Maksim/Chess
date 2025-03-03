using Backend.Models.Request;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Produces("application/json")]
[Route("api-chess/v{version:apiVersion}/[controller]")]
public class AuthController(AuthService authService): ControllerBase
{
    [AllowAnonymous]
    [MapToApiVersion("1.0")]
    [HttpPost("registration")]
    public async Task<IActionResult> RegistrationUser(RegistrationUser user)
    {
        var result = await authService.RegisterUserAsync(user);
        
        if (!result.Success)
            return StatusCode(result.StatusCode, result.Message);
        
        return Ok(result);
    }
    
    [AllowAnonymous]
    [MapToApiVersion("1.0")]
    [HttpPost("authorization")]
    public async Task<IActionResult> AuthorizationUser(AuthUser user)
    {
        var result = await authService.LoginUserAsync(user);
        
        if (!result.Success)
            return StatusCode(result.StatusCode, result.Message);
        
        return Ok(result);
    }
}