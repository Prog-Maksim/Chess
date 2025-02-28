using System.IO;

namespace Frontend.Scrypt;

public class SaveRepository
{
    private const string filePath = "token.txt";
    
    public static bool SaveToken(string token)
    {
        File.WriteAllText(filePath, token);
        return true;
    }

    public static string ReadToken()
    {
        return File.ReadAllText(filePath);
    }

    public static bool CheckToken()
    {
        return File.Exists(filePath);
    }
}