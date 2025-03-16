using Backend.Enums;
using Backend.Game.Shapes;

namespace Backend.Game.Potion;

public interface IPotion
{
    string Name { get; }
    PotionType Type { get; }
    string Description { get; }
    int PurchasePrice { get; }
    int UnlockPrice { get; }
    int UnlockLevel { get; }

    /// <summary>
    /// Применения зелья
    /// </summary>
    /// <param name="game">Объект игры</param>
    /// <param name="chessPlayer">Объект игрока</param>
    /// <param name="targetPiece">Объект фигуры</param>
    Task ApplyEffect(BaseChessGame game, ChessPlayer chessPlayer, ChessPiece? targetPiece);
    
    /// <summary>
    /// Проверяет разрешены ли зелья
    /// </summary>
    /// <param name="game">Объект игры</param>
    /// <returns>Разрешены ли зелья</returns>
    bool CheckPotionAllowed(BaseChessGame game);
}