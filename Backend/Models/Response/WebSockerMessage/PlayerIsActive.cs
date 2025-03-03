﻿namespace Backend.Models.Response.WebSockerMessage;

public class PlayerIsActive: BaseWebSocketMessage
{
    public required string PlayerId { get; set; }
    public bool State { get; set; }
    public TimeSpan Time { get; set; }
}