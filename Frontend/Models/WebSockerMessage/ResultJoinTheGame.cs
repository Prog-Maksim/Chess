﻿namespace Frontend.Models.WebSockerMessage;

/// <summary>
/// Результат ответа, на приглашение в игру
/// </summary>
public class ResultJoinTheGame: BaseWebSocketMessage
{
    public bool Status { get; set; }
}