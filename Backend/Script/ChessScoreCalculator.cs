using Backend.Enums;
using Backend.Game;
using Backend.Game.GameModes;
using Backend.Models.DB;

namespace Backend.Script;

public class ChessScoreCalculator
{
    /// <summary>
    /// Рассчитывает количество очков за победу в зависимости от типа игры и оставшегося времени.
    /// </summary>
    /// <param name="gameType">Тип игры (Классика, Рапид, Блиц, Буллит)</param>
    /// <param name="timeLeftSeconds">Оставшееся время игрока в секундах</param>
    /// <param name="playerCount">Кол-во игроков</param>
    /// <returns>Количество начисленных очков</returns>
    public static int CalculateScore(IGameMode gameType, TimeSpan timeLeftSeconds, int playerCount)
    {
        if (playerCount < 2)
            throw new ArgumentException("Количество игроков должно быть не менее 2.", nameof(playerCount));

        
        int baseScore = gameType.GameMode switch
        {
            GameMode.Classic => 10,
            GameMode.Rapid     => 8,
            GameMode.Blitz     => 6,
            GameMode.Bullet    => 4,
            _ => 0
        };
        
        if (timeLeftSeconds.TotalSeconds <= 0)
            return baseScore;

        // Бонусные очки за оставшееся время (например, 1 очко за каждые 1 минуту)
        int timeBonus = (int)timeLeftSeconds.TotalSeconds / 60;
        
        // Коэффициент сложности: если больше 2 игроков, бонус выше
        double difficultyMultiplier = playerCount > 2 ? 1.5 : 1.0;

        return (int)((baseScore + timeBonus) * difficultyMultiplier);
    }

    /// <summary>
    /// Рассчитывает получаемое кол-во рейтинга за игру
    /// </summary>
    /// <param name="isWinner"></param>
    /// <param name="playerData"></param>
    /// <param name="playerCount"></param>
    /// <param name="leagues"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="Exception"></exception>
    public static int CalculateRatingChange(UserData playerData, int playerCount, bool isWinner, List<League> leagues)
    {
        if (playerCount < 2)
            throw new ArgumentException("Количество игроков должно быть не менее 2.", nameof(playerCount));

        // Ищем текущую лигу игрока
        var currentLeague = leagues.FirstOrDefault(l => l.LeagueName == playerData.League);
        if (currentLeague == null)
            throw new Exception($"Лига '{playerData.League}' не найдена.");

        // Базовое значение рейтинга
        int baseRating = currentLeague.RatingIsWinner;
        Console.WriteLine($"Базовый рейтинг: {baseRating}");

        if (!isWinner)
        {
            Console.WriteLine("Игрок проиграл!");
            if (playerData.Rating < baseRating)
                return -playerData.Rating;
            
            return -baseRating;
        }

        // Коэффициент сложности (чем больше игроков, тем выше бонус)
        double difficultyMultiplier = 1.0 + (playerCount - 2) * 0.1;
        Console.WriteLine($"Коэффицент сложности: {difficultyMultiplier}");

        // Итоговый рейтинг с округлением
        int result = (int)Math.Round(baseRating * difficultyMultiplier);
        Console.WriteLine($"Итоговый рассчет: {result}");
        return result;
    }
}