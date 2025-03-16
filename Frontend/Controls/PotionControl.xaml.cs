using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Frontend.Enums;
using Frontend.Script;
using Frontend.Windows.Game;

namespace Frontend.Controls;

public partial class PotionControl : UserControl
{
    private string _potionId;
    private PotionType _potionType;
    private int _count;
    private bool _isPurchased = false;
    private bool _isUnlocked = false;
    
    public PotionControl()
    {
        InitializeComponent();
    }
    
    public PotionControl(string potionId, string name, string description, int price, PotionType type, int levelUnlock): this()
    {
        Title.Text = name;
        Description.Text = description;
        
        _potionType = type;
        _potionId = potionId;
        
        BlockElement(levelUnlock);
    }

    private BitmapImage GetPotionImage()
    {
        string basePath = AppDomain.CurrentDomain.BaseDirectory;
        string fullPath = Path.Combine(basePath, "Image", "Potion", $"{_potionType}.png");

        return new BitmapImage(new Uri(fullPath, UriKind.Absolute));
    }

    private void BlockElement(int levelUnlock)
    {
        BitmapImage originalImage = GetPotionImage();
        
        // Создаем затемненное изображение
        DrawingGroup drawingGroup = new DrawingGroup();
        using (DrawingContext dc = drawingGroup.Open())
        {
            // Затемняем основное изображение (прозрачность 60%)
            ImageBrush imageBrush = new ImageBrush(originalImage)
            {
                Opacity = 0.4 // 40% видимости, т.е. затемнение на 60%
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

        // Устанавливаем итоговое изображение в контрол
        DrawingImage finalImage = new DrawingImage(drawingGroup);
        ImagePotion.Source = finalImage;
        
        MainButton.IsEnabled = false;
        MainButton.Content = $"Доступно с {levelUnlock} уровня";
    }
    
    private void RestoreOriginalImage()
    {
        if (ImagePotion == null) return;
        ImagePotion.Source = GetPotionImage();
    }

    public void UnlockPotion(int count, bool isPurchased, bool isUnlocked)
    {
        if (isUnlocked)
            RestoreOriginalImage();

        if (isPurchased)
        {
            BorderCount.Visibility = Visibility.Visible;
            CountText.Text = count.ToString();
        }

        _count = count;
        _isPurchased = isPurchased;
        _isUnlocked = isUnlocked;

        if (isUnlocked)
        {
            MainButton.IsEnabled = true;
            
            if (!isPurchased)
                MainButton.Content = "Разблокировать";
            else
                MainButton.Content = "Купить";
        }
    }

    private void MainButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (!_isPurchased)
        {
            MainButton.IsEnabled = false;
            MainButton.Content = "Загрузка";
            
            _ = PurchasePotion();
        }
        else
        {
            MainButton.IsEnabled = false;
            MainButton.Content = "Загрузка";
            
            _ = BuyPotion();
        }
    }


    private async Task BuyPotion()
    {
        using HttpClient client = new HttpClient();
        
        var requestUrl = Url.BaseUrl + $"Potion/buy-potion?potionId={_potionId}";
        
        using var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", $"{SaveRepository.ReadToken()}");
        
        var response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            bool result = Convert.ToBoolean(responseContent);

            if (result)
                UnlockPotion(++_count, true, true);
        }
        
        MainButton.IsEnabled = true;
        MainButton.Content = "Купить";
    }

    private async Task PurchasePotion()
    {
        using HttpClient client = new HttpClient();
        
        var requestUrl = Url.BaseUrl + $"Potion/unlock-potion?potionId={_potionId}";
        
        using var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", $"{SaveRepository.ReadToken()}");
        
        var response = await client.SendAsync(request);
        
        if (response.IsSuccessStatusCode)
        {
            MainButton.IsEnabled = true;
            MainButton.Content = "Купить";
            
            var responseContent = await response.Content.ReadAsStringAsync();
            bool result = Convert.ToBoolean(responseContent);

            if (result)
                UnlockPotion(0, true, true);
            return;
        }
        
        MainButton.IsEnabled = true;
        MainButton.Content = "Разблокировать";
    }
}