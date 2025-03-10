using System.ComponentModel.DataAnnotations;
using Backend.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Produces("application/json")]
[Route("api-chess/v{version:apiVersion}/[controller]")]
public class ImageController: ControllerBase
{
    private readonly string _basePath = "Image/Base";
    
    /// <summary>
    /// Выдает изображения фигур
    /// </summary>
    /// <param name="type">Тип фигуры</param>
    /// <returns></returns>
    /// <response code="200">Изображение в формате svg</response>
    /// <response code="404">Данное изображение не найдено</response>
    [AllowAnonymous]
    [HttpGet("get-image")]
    [ProducesResponseType(typeof(FileStreamResult),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFound), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetImage([Required][FromQuery] PieceType type)
    {
        string fileName = $"{char.ToUpper(type.ToString()[0])}{type.ToString().Substring(1)}.svg";
        string filePath = Path.Combine(_basePath, fileName);
        
        if (!System.IO.File.Exists(filePath))
            return NotFound("Файл не найден");
        
        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return File(fileStream, "image/svg+xml");
    }
}