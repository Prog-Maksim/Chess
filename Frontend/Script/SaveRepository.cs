using System.IO;

namespace Frontend.Script;

public class SaveRepository
{
    private const string filePath = "token.txt";
    private const string fileIdPath = "id.txt";
    
    public static bool SaveToken(string token)
    {
        File.WriteAllText(filePath, token);
        return true;
    }

    public static bool SaveId(string id)
    {
        File.WriteAllText(fileIdPath, id);
        return true;
    }

    public static string ReadToken()
    {
        return File.ReadAllText(filePath);
    }

    public static string ReadId()
    {
        return File.ReadAllText(fileIdPath);
    }

    public static void DeleteSave()
    {
        File.Delete(filePath);
        File.Delete(fileIdPath);
    }

    public static bool CheckToken()
    {
        return File.Exists(filePath);
    }

    public static bool CheckId()
    {
        return File.Exists(fileIdPath);
    }
}