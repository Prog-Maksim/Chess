﻿namespace Backend.Models.Response.WebSocketMessage;

public class RemovePlayer: BaseWebSocketMessage
{
    public required string PlayerId { get; set; }
}