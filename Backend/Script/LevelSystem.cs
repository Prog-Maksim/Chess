namespace Backend.Script;

public class LevelSystem
{
    // Коэффициент роста
    private const double A = 7.0; 
    // Минимальное количество побед для базового уровня
    private const double B = 1.0; 

    public static int WinsRequiredForLevel(int level)
    {
        if (level <= 1)
            return 0;
        
        return (int)Math.Ceiling(A * Math.Log(level - 1) + B + 2);
    }
}