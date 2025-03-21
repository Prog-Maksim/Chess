﻿using System.Text.Json.Serialization;

namespace Frontend.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GameState
{
    /// <summary>
    /// Ожидание игроков
    /// </summary>
    WaitingForPlayers,
    /// <summary>
    /// Ожидание начала игры
    /// </summary>
    Countdown,
    /// <summary>
    /// Игра идет
    /// </summary>
    InProgress,
    /// <summary>
    /// Игра завершена
    /// </summary>
    Finished,
    /// <summary>
    /// Игра остановлена
    /// </summary>
    Stopped
}