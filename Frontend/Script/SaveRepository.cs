using System.IO;
using System.Text.Json;
using Frontend.Models;

namespace Frontend.Script;

public class SaveRepository
{
    public const string filePath = "data.json";

    public static void SaveDataToFile(Token token)
    {
        string json = JsonSerializer.Serialize(token);
        File.WriteAllText(filePath, json);
    }

    public static Token LoadTokenFromFile()
    {
        string json = File.ReadAllText(filePath);
        Token? token = JsonSerializer.Deserialize<Token>(json);

        if (token == null)
            throw new NullReferenceException("Токен не найден");
        
        return token;
    }
    
    public static bool FileExists()
    {
        return File.Exists(filePath);
    }
    
    public static void DeleteFile()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Console.WriteLine("Файл успешно удален.");
        }
        else
        {
            Console.WriteLine("Файл не найден.");
        }
    }
}