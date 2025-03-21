﻿using Backend.Enums;

namespace Backend.Game.Shapes;

/// <summary>
/// Игровая фигура
/// </summary>
public abstract class ChessPiece
{
    public int Score { get; private set; }
    
    public string ChessPieceId { get; set; } = Guid.NewGuid().ToString();
    public bool IsFirstMove { get; set; } = true;
    public PieceType Type { get; }
    public string OwnerId { get; set; }
    
    public int Row { get; set; }
    public int Column { get; set; }

    public int DurationThePotion { get; set; } = 0;
    public bool IsOriginalEnlarged { get; set; }
    public bool IsEnlarged { get; set; } = false;
    public bool IsProtected { get; set; } = false;
    
    
    public ChessPiece(PieceType type, string ownerId, int score)
    {
        Score = score;
        Type = type;
        OwnerId = ownerId;
    }
    
    /// <summary>
    /// Базовые правила движения фигуры
    /// </summary>
    /// <param name="newRow"></param>
    /// <param name="newCol"></param>
    /// <returns></returns>
    public abstract bool IsValidMove(int newRow, int newCol);
}