﻿using System.Text.Json.Serialization;

namespace Backend.Models.Response;

public class Token: BaseResponse
{
    /// <summary>
    /// Access токен для доступа к сайту
    /// </summary>
    public required string AccessToken { get; set; }
}