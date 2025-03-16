using System.IO;
using System.Net.Http;
using Frontend.Enums;

namespace Frontend.Script;

public class ProgramInitializer
{
    public static void DirectoryCreator()
    {
        if (!Directory.Exists("Image"))
            Directory.CreateDirectory("Image");
        
        if (!Directory.Exists("Image/Temporary"))
            Directory.CreateDirectory("Image/Temporary");
        
        if (!Directory.Exists("Image/Static"))
            Directory.CreateDirectory("Image/Static");

        _ = InstallBaseImage();
        _ = InstallImagePotion();
    }

    private const string BaseUrl = Url.BaseUrl + "Image/get-image?type=";
    private const string SaveDirectory = "Image/Static/Piece/Base";
    
    private const string BaseUrlPotion = Url.BaseUrl + "Image/get-image-potion?type=";
    private const string SaveDirectoryPotion = "Image/Potion";

    private static async Task InstallBaseImage()
    {
        if (!Directory.Exists(SaveDirectory))
            Directory.CreateDirectory(SaveDirectory);
        
        using (HttpClient client = new HttpClient())
        {
            foreach (PieceType type in Enum.GetValues(typeof(PieceType)))
            {
                string url = BaseUrl + type;
                string savePath = Path.Combine(SaveDirectory, $"{type}.svg");
                
                if (File.Exists(savePath))
                    continue;

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    await using FileStream fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None);
                    await response.Content.CopyToAsync(fileStream);

                    Console.WriteLine($"Файл сохранён: {savePath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при скачивании {type}: {ex.Message}");
                }
            }
        }
        
        PieceConversion();
    }

    private static async Task InstallImagePotion()
    {
        if (!Directory.Exists(SaveDirectoryPotion))
            Directory.CreateDirectory(SaveDirectoryPotion);
        
        using (HttpClient client = new HttpClient())
        {
            foreach (PotionType type in Enum.GetValues(typeof(PotionType)))
            {
                string url = BaseUrlPotion + type;
                string savePath = Path.Combine(SaveDirectoryPotion, $"{type}.png");
                
                if (File.Exists(savePath))
                    continue;

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    await using FileStream fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None);
                    await response.Content.CopyToAsync(fileStream);

                    Console.WriteLine($"Файл сохранён: {savePath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при скачивании {type}: {ex.Message}");
                }
            }
        }
    }

    private static void PieceConversion()
    {
        string[] colors = { "#000000", "#eeeeee", "#7074D5", "#DD4CEE" };
        
        foreach (PieceType type in Enum.GetValues(typeof(PieceType)))
        {
            string savePath = Path.Combine(SaveDirectory, $"{type}.svg");
            
            foreach (var color in colors)
            {
                Rasterization.SvgToPng(savePath, color);
            }
        }
    }
}