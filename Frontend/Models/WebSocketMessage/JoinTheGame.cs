﻿namespace Frontend.Models.WebSocketMessage;

/// <summary>
/// Заявка на вступление в игру
/// </summary>
public class JoinTheGame: BaseWebSocketMessage
{
    public required string Nickname { get; set; }
    public required string PersonId { get; set; }
}