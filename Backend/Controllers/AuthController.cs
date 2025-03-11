using Backend.Models.Request;
using Backend.Models.Response;
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
    /// <summary>
    /// Регистрация пользователя
    /// </summary>
    /// <param name="user">Информация о пользователе</param>
    /// <returns></returns>
    /// <response code="200">Успешная регистрация нового пользователя</response>
    /// <response code="403">Данный пользователь уже существует</response>
    /// <response code="400">Невалидные данные</response>
    [AllowAnonymous]
    [MapToApiVersion("1.0")]
    [HttpPost("registration")]
    [ProducesResponseType(typeof(Token), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegistrationUser(RegistrationUser user)
    {
        var result = await authService.RegisterUserAsync(user);
        
        if (!result.Success)
            return StatusCode(result.StatusCode, result);
        
        return Ok(result);
    }
    
    /// <summary>
    /// Авторизация пользователя
    /// </summary>
    /// <param name="user">Информация о пользователе</param>
    /// <returns></returns>
    /// <response code="200">Успешная авторизация пользователя</response>
    /// <response code="403">Логин или пароль не верен!</response>
    /// <response code="404">Данный пользователь не найден</response>
    /// <response code="400">Невалидные данные</response>
    [AllowAnonymous]
    [MapToApiVersion("1.0")]
    [HttpPost("authorization")]
    [ProducesResponseType(typeof(Token), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AuthorizationUser(AuthUser user)
    {
        var result = await authService.LoginUserAsync(user);
        
        if (!result.Success)
            return StatusCode(result.StatusCode, result);
        
        return Ok(result);
    }


    /// <summary>
    /// Обновление пароля
    /// </summary>
    /// <param name="password">Информация о пароле</param>
    /// <returns></returns>
    /// <response code="200">Успешное обновление пароля</response>
    /// <response code="403">Пароль не верен</response>
    /// <response code="404">Данный пользователь не найден</response>
    /// <response code="400">Невалидные данные</response>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPut("password")]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePassword(UpdatePassword password)
    {
        var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
        var token = authHeader.Substring("Bearer ".Length);
        var dataToken = JwtService.GetJwtTokenData(token);

        var result =
            await authService.UpdatePasswordAsync(dataToken.PersonId, password.OldPassword, password.NewPassword);

        if (!result.Success)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }
}