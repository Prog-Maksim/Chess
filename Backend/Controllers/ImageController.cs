using System.ComponentModel.DataAnnotations;
using Backend.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Produces("application/json")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ImageController: ControllerBase
{
    private readonly string _basePath = "Image/Base";
    
    [AllowAnonymous]
    [HttpGet("get-image")]
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