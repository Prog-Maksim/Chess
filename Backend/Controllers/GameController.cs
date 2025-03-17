using System.ComponentModel.DataAnnotations;
using System.Net.WebSockets;
using Backend.Enums;
using Backend.Filters;
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
public class GameController(WebSocketService webSocketService, GameService gameService, ILogger<GameController> logger, PotionService _potionService): ControllerBase
{
    /// <summary>
    /// Подключение пользователей по WebSocket
    /// </summary>
    /// <returns></returns>
    /// <response code="200">Успешное подключение</response>
    /// <response code="400">Запрос не является Web Socket</response>
    [Authorize]
    [HttpGet("connect")]
    [ServiceFilter(typeof(ValidateJwtAccessTokenFilter))]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status400BadRequest)]
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
        return BadRequest(new BaseResponse
        {
            Success = false,
            StatusCode = 400,
            Error = "Bad Request",
            Message = "Запрос не является Web Socket"
        });
    }

    /// <summary>
    /// Создание игры
    /// </summary>
    /// <param name="name">Название игры</param>
    /// <param name="players">Кол-во игроков</param>
    /// <param name="isPotion">Разрешены ли зелья</param>
    /// <param name="isPrivate">Приватная ли игра</param>
    /// <param name="mode">Тип игры</param>
    /// <returns></returns>
    /// <response code="200">Успешное создание игры</response>
    [Authorize]
    [HttpPost("create-game")]
    [ServiceFilter(typeof(ValidateJwtAccessTokenFilter))]
    [ProducesResponseType(typeof(CreateGame), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateGame(
        [Required][FromQuery] string name, 
        [Required][FromQuery] int players, 
        [Required][FromQuery] bool isPotion, 
        [Required][FromQuery] bool isPrivate, 
        GameMode mode = GameMode.Rapid)
    {
        var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
        var token = authHeader.Substring("Bearer ".Length);
        var dataToken = JwtService.GetJwtTokenData(token);
        
        string gameId = gameService.CreateGame(name, players, dataToken.PersonId, dataToken.Nickname, isPrivate, isPotion, mode);
        
        return Ok(new CreateGame
        {
            Success = true,
            Message = $"Игра `{name}` успешно создана",
            GameId = gameId
        });
    }
    
    /// <summary>
    /// Оставляет заявку на вступление в игру
    /// </summary>
    /// <param name="gameId">Идентификатор игры</param>
    /// <returns></returns>
    /// <response code="200">Успешно</response>
    /// <response code="404">Данная игра не найдена</response>
    /// <response code="400">Пользователь не подключен к Web Socket</response>
    [Authorize]
    [HttpPost("login-game")]
    [ServiceFilter(typeof(ValidateJwtAccessTokenFilter))]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LogInGame(string gameId)
    {
        var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
        var token = authHeader.Substring("Bearer ".Length);
        var dataToken = JwtService.GetJwtTokenData(token);

        if (webSocketService.GetWebSocket(dataToken.PersonId) == null)
        {
            return StatusCode(400, new BaseResponse
            {
                Success = false,
                Message = "Пользователь не подключен к игре",
                Error = "Bad Request",
                StatusCode = 400
            });
        }
        
        BaseResponse result = await gameService.JoinGame(gameId, dataToken.PersonId, dataToken.Nickname);
        
        if (!result.Success)
            return StatusCode(result.StatusCode, result);
        
        return Ok(result);
    }

    /// <summary>
    /// Возвращает игровое поле
    /// </summary>
    /// <param name="gameId">Идентификатор игры</param>
    /// <returns></returns>
    /// <response code="200">Успешно</response>
    /// <response code="404">Данная игра или игрок не найдена</response>
    /// <response code="403">У игрока нет доступа к этой игре</response>
    [Authorize]
    [HttpGet("playing-field")]
    [ServiceFilter(typeof(ValidateJwtAccessTokenFilter))]
    [ProducesResponseType(typeof(ContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetPlayingField(string gameId)
    {
        var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
        var token = authHeader.Substring("Bearer ".Length);
        var dataToken = JwtService.GetJwtTokenData(token);

        try
        {
            var result = await _potionService.GetPersonData(dataToken.PersonId);
            var board = await gameService.GetBoard(gameId, dataToken.PersonId, result);
            var boardStr = JsonConvert.SerializeObject(board);

            return new ContentResult
            {
                Content = boardStr,
                ContentType = "application/json",
                StatusCode = 200
            };
        }
        catch (KeyNotFoundException e)
        {
            return StatusCode(404, new BaseResponse
            {
                Success = false,
                StatusCode = 404,
                Message = e.Message,
                Error = "Not Found"
            });
        }
        catch (NullReferenceException e)
        {
            return StatusCode(404, new BaseResponse
            {
                Success = false,
                StatusCode = 404,
                Message = e.Message,
                Error = "Not Found"
            });
        }
        catch (UnauthorizedAccessException e)
        {
            return StatusCode(403, new BaseResponse
            {
                Success = false,
                StatusCode = 403,
                Message = e.Message,
                Error = "Forbidden"
            });
        }
    }

    /// <summary>
    /// Разрешить игроку вступить в игру
    /// </summary>
    /// <param name="gameId">Идентификатор игры</param>
    /// <param name="personId">Идентификатор игрока</param>
    /// <returns></returns>
    /// <response code="200">Статус выполнения</response>
    /// <response code="404">Данная игра не найдена</response>
    [Authorize]
    [HttpPost("approve-player")]
    [ServiceFilter(typeof(ValidateJwtAccessTokenFilter))]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApprovePlayerTheGame(string gameId, string personId)
    {
        var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
        var token = authHeader.Substring("Bearer ".Length);
        var dataToken = JwtService.GetJwtTokenData(token);

        try
        {
            var res = await gameService.ApprovePersonTheGame(gameId, personId, dataToken.PersonId);
            return Ok(res);
        }
        catch (KeyNotFoundException e)
        {
            return StatusCode(404, new BaseResponse
            {
                Success = false,
                StatusCode = 404,
                Error = "Not Found",
                Message = e.Message
            });
        }
    }
    
    /// <summary>
    /// Запретить пользователю вступать в игру
    /// </summary>
    /// <param name="gameId">Идентификатор игры</param>
    /// <param name="personId">Идентификатор игрока</param>
    /// <returns></returns>
    /// <response code="200">Статус выполнения</response>
    /// <response code="404">Данная игра не найдена</response>
    [Authorize]
    [HttpPost("reject-player")]
    [ServiceFilter(typeof(ValidateJwtAccessTokenFilter))]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RejectPlayerTheGame(string gameId, string personId)
    {
        var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
        var token = authHeader.Substring("Bearer ".Length);
        var dataToken = JwtService.GetJwtTokenData(token);

        try
        {
            var res = await gameService.RejectPersonTheGame(gameId, personId, dataToken.PersonId);
            return Ok(res);
        }
        catch (KeyNotFoundException e)
        {
            return StatusCode(404, new BaseResponse
            {
                Success = false,
                StatusCode = 404,
                Error = "Not Found",
                Message = e.Message
            });
        }
    }
    
    /// <summary>
    /// Позволяет игроку выйти из игры
    /// </summary>
    /// <param name="gameId">Идентификатор игры</param>
    /// <returns></returns>
    /// <response code="200">Успешно</response>
    /// <response code="404">Данная игра не найдена</response>
    [Authorize]
    [HttpPost("leave-game")]
    [ServiceFilter(typeof(ValidateJwtAccessTokenFilter))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ExitTheGame(string gameId)
    {
        var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
        var token = authHeader.Substring("Bearer ".Length);
        var dataToken = JwtService.GetJwtTokenData(token);

        try
        {
            await gameService.LeaveTheGame(gameId, dataToken.PersonId);
            return Ok();
        }
        catch (KeyNotFoundException e)
        {
            return StatusCode(404, new BaseResponse
            {
                Success = false,
                StatusCode = 404,
                Error = "Not Found",
                Message = e.Message
            });
        }
    }
}