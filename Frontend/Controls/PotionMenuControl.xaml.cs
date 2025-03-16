using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Frontend.Enums;
using Frontend.Models;
using Frontend.Script;
using Frontend.Windows.Game;

namespace Frontend.Controls;

public partial class PotionMenuControl : UserControl
{
    private readonly GameMenu _gameMenu;
    
    public PotionMenuControl()
    {
        InitializeComponent();
    }
    
    private BitmapImage GetPotionImage(PotionType type)
    {
        string basePath = AppDomain.CurrentDomain.BaseDirectory;
        string fullPath = Path.Combine(basePath, "Image", "Potion", $"{type}.png");

        return new BitmapImage(new Uri(fullPath, UriKind.Absolute));
    }

    public PotionMenuControl(GameMenu menu, PotionAvailable potionAvailable): this()
    {
        _gameMenu = menu;
        
        for (int i = 0; i < potionAvailable.Potions.Count; i++)
        {
            var potion = potionAvailable.Potions[i];
            
            if (i == 0)
            {
                if (!potion.IsUnlocked)
                {
                    AddlockImage(Image1, potion.Type);
                }
                else if (!potion.IsAvailable)
                {
                    BlackoutImage(Image1, potion.Type);
                }
                else
                {
                    Image1.Source = GetPotionImage(potion.Type);
                    Border1.MouseLeftButtonDown += (sender, args) =>
                    {
                        Console.WriteLine($"Potion type: {potion.Type}");
                        _ = UsePotionAsync(potion.Type);
                    };
                }
            }
            if (i == 1)
            {
                if (!potion.IsUnlocked)
                {
                    AddlockImage(Image2, potion.Type);
                }
                else if (!potion.IsAvailable)
                {
                    BlackoutImage(Image2, potion.Type);
                }
                else
                {
                    Image2.Source = GetPotionImage(potion.Type);
                    Border2.MouseLeftButtonDown += (sender, args) =>
                    {
                        Console.WriteLine($"Potion type: {potion.Type}");
                        _ = UsePotionAsync(potion.Type);
                    };
                }
            }
            if (i == 2)
            {
                if (!potion.IsUnlocked)
                {
                    AddlockImage(Image3, potion.Type);
                }
                else if (!potion.IsAvailable)
                {
                    BlackoutImage(Image3, potion.Type);
                }
                else
                {
                    Image3.Source = GetPotionImage(potion.Type);
                    Border3.MouseLeftButtonDown += (sender, args) =>
                    {
                        Console.WriteLine($"Potion type: {potion.Type}");
                        _ = UsePotionAsync(potion.Type);
                    };
                }
            }
            if (i == 3)
            {
                if (!potion.IsUnlocked)
                {
                    AddlockImage(Image4, potion.Type);
                }
                else if (!potion.IsAvailable)
                {
                    BlackoutImage(Image4, potion.Type);
                }
                else
                {
                    Image4.Source = GetPotionImage(potion.Type);
                    Border4.MouseLeftButtonDown += (sender, args) =>
                    {
                        Console.WriteLine($"Potion type: {potion.Type}");
                        _ = UsePotionAsync(potion.Type);
                    };
                }
            }
            if (i == 4)
            {
                if (!potion.IsUnlocked)
                {
                    AddlockImage(Image5, potion.Type);
                }
                else if (!potion.IsAvailable)
                {
                    BlackoutImage(Image5, potion.Type);
                }
                else
                {
                    Image5.Source = GetPotionImage(potion.Type);
                    Border5.MouseLeftButtonDown += (sender, args) =>
                    {
                        Console.WriteLine($"Potion type: {potion.Type}");
                        _ = UsePotionAsync(potion.Type);
                    };
                }
            }
        }
    }
    
    private void AddlockImage(Image image, PotionType type)
    {
        BitmapImage originalImage = GetPotionImage(type);
        
        DrawingGroup drawingGroup = new DrawingGroup();
        using (DrawingContext dc = drawingGroup.Open())
        {
            ImageBrush imageBrush = new ImageBrush(originalImage)
            {
                Opacity = 0.25 
            };
            dc.DrawRectangle(imageBrush, null, new Rect(0, 0, originalImage.Width, originalImage.Height));

            // Загружаем изображение замка
            BitmapImage lockImage = new BitmapImage(new Uri("pack://application:,,,/Image/Lock.png"));

            double lockWidth = originalImage.Width * 0.55;
            double lockHeight = originalImage.Height * 0.6;

            // Позиция замка по центру изображения
            double posX = (originalImage.Width - lockWidth) / 2;
            double posY = (originalImage.Height - lockHeight) / 2;

            // Рисуем уменьшенное изображение замка в центре
            dc.DrawImage(lockImage, new Rect(posX, posY, lockWidth, lockHeight));
        }
        
        DrawingImage finalImage = new DrawingImage(drawingGroup);
        image.Source = finalImage;
    }

    private void BlackoutImage(Image image, PotionType type)
    {
        BitmapImage originalImage = GetPotionImage(type);
        
        DrawingGroup drawingGroup = new DrawingGroup();
        using (DrawingContext dc = drawingGroup.Open())
        {
            ImageBrush imageBrush = new ImageBrush(originalImage)
            {
                Opacity = 0.25
            };
            dc.DrawRectangle(imageBrush, null, new Rect(0, 0, originalImage.Width, originalImage.Height));
        }
        
        DrawingImage finalImage = new DrawingImage(drawingGroup);
        image.Source = finalImage;
    }
    
    public async Task UsePotionAsync(PotionType type)
    {
        var pos = _gameMenu.StartPoint;
        
        using HttpClient client = new HttpClient();
        
        string requestUrl;
        if (pos != null)
            requestUrl = $"http://localhost:5064/api-chess/v1/Potion/use-potion?gameId={_gameMenu.GameId}&potionType={type}&row={pos.Row}&column={pos.Col}";
        else
            requestUrl = $"http://localhost:5064/api-chess/v1/Potion/use-potion?gameId={_gameMenu.GameId}&potionType={type}";
            

        using var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
        request.Headers.Add("Authorization", $"Bearer {SaveRepository.ReadToken()}");
        
        HttpResponseMessage response = await client.SendAsync(request);
        var result = await response.Content.ReadAsStringAsync();
        Console.WriteLine(result);
    }
}