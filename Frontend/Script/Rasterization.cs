using System.IO;
using System.Text;
using ImageMagick;

namespace Frontend.Script;

public static class Rasterization
{
    static readonly string basicColor = "#000000";
    
    public static void SvgToPng(string pathToImage, string color)
    {
        if (!Directory.Exists($"Image/Temporary/{color}"))
            Directory.CreateDirectory($"Image/Temporary/{color}");
        
        string name = pathToImage;
        name = name.Split('\\').Last().Replace(".svg", "");
        
        string pathToSave = $"Image/Temporary/{color}/{name}.png";
        
        if (File.Exists(pathToSave))
            return;

        string svgImage = File.ReadAllText(pathToImage);
        svgImage = svgImage.Replace(basicColor, color);

        byte[] bytes = Encoding.ASCII.GetBytes(svgImage);
        
        var magickReadSettings = new MagickReadSettings { Format = MagickFormat.Svg };
        
        using var image = new MagickImage(bytes, magickReadSettings) { Format = MagickFormat.Png };
        
        image.Alpha(AlphaOption.Set);
        image.Transparent(new MagickColor("#FFFFFF"));
        image.Format = MagickFormat.Png32;
        
        image.Write(pathToSave);
    }
}