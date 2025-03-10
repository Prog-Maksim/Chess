using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Produces("application/json")]
[Route("api-chess/v{version:apiVersion}/[controller]")]
public class VersioningController: ControllerBase
{
    private const string ApiVersion = "[ALPHA] 01.00.000";
    private static readonly HashSet<string> AllowedTags = new() { "BETA", "ALPHA", "RELEASE" };
    
    /// <summary>
    /// Проверяет версию клиента
    /// </summary>
    /// <param name="version">Версия в формате: [Tag] xx.xx.xxx</param>
    /// <returns></returns>
    /// <response code="200">Результат, сходятся ли версии</response>
    /// <response code="400">Некорректный формат версии</response>
    /// <response code="500">Ошибка версии на сервере</response>
    [AllowAnonymous]
    [HttpGet("check-version")]
    [ProducesResponseType(typeof(BadRequest),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckVersionClient([Required][FromQuery] string version)
    {
        if (!TryParseVersion(version, out var clientTag, out var clientMajor, out var clientMinor))
            return BadRequest("Некорректный формат версии.");
        
        if (!TryParseVersion(ApiVersion, out var apiTag, out var apiMajor, out var apiMinor))
            return StatusCode(500, "Ошибка конфигурации API версии.");

        if (clientTag != apiTag)
            return BadRequest("Версия содержит неверный Tag.");

        bool isVersionMatch = clientMajor == apiMajor && clientMinor == apiMinor;
        return Ok(isVersionMatch);
    }
    
    private static bool TryParseVersion(string version, out string tag, out int major, out int minor)
    {
        tag = string.Empty;
        major = minor = 0;

        var match = Regex.Match(version, @"^\[(\w+)\]\s(\d{2})\.(\d{2})\.\d{3}$");
        if (!match.Success) return false;

        tag = match.Groups[1].Value;
        if (!AllowedTags.Contains(tag)) return false;

        major = int.Parse(match.Groups[2].Value);
        minor = int.Parse(match.Groups[3].Value);

        return true;
    }
}