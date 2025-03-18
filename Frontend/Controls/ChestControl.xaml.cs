using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Backend.Models.Response;
using Frontend.Script;

namespace Frontend.Controls;

public partial class ChestControl : UserControl
{
    private bool _isChest;
    
    public ChestControl()
    {
        InitializeComponent();

        BlackoutImage();
    }

    public void StateChest(bool isChest, int max, int current)
    {
        _isChest = isChest;
        
        if (isChest)
        {
            ChestProgressBar.Visibility = Visibility.Hidden;
            ChestProgressBar.Height = 0;
            
            MainBorder.Background = (Brush)new BrushConverter().ConvertFrom("#FFE066");
            MainBorder.Cursor = Cursors.Hand;
        }
        else
        {
            BlackoutImage();
            ChestProgressBar.Visibility = Visibility.Visible;
            ChestProgressBar.Height = 20;
            
            ChestProgressBar.Maximum = max;
            ChestProgressBar.Value = current;
            ChestProgressBar.Tag = $"{current}/{max}";
            MainBorder.Background = (Brush)new BrushConverter().ConvertFrom("#FFEA94");
            MainBorder.Cursor = Cursors.Arrow;
        }
    }

    private void MainBorder_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (_isChest)
        {
            _ = OpenChestAsync();
        }
    }

    private void BlackoutImage()
    {
        BitmapImage image = new BitmapImage(new Uri("pack://application:,,,/Image/chest.png"));
        
        DrawingGroup drawingGroup = new DrawingGroup();
        using (DrawingContext dc = drawingGroup.Open())
        {
            ImageBrush imageBrush = new ImageBrush(image)
            {
                Opacity = 0.7
            };
            dc.DrawRectangle(imageBrush, null, new Rect(0, 0, image.Width, image.Height));
        }
        
        DrawingImage finalImage = new DrawingImage(drawingGroup);
        ChestImage.Source = finalImage;
    }
    
    private async Task OpenChestAsync()
    {
        using HttpClient client = new HttpClient();
        string url = Url.BaseUrl + "Profile/chest";
        
        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        
        request.Headers.Add("Accept", "application/json");
        request.Headers.Add("Authorization", $"Bearer {SaveRepository.LoadTokenFromFile().AccessToken}");
        
        request.Content = new StringContent("", Encoding.UTF8, "application/json");
        HttpResponseMessage response = await client.SendAsync(request);
        var result = await response.Content.ReadAsStringAsync();
        MessageBox.Show(result);

        if (response.IsSuccessStatusCode)
        {
            ChestReward? reward = JsonSerializer.Deserialize<ChestReward>(result);
        }
        else
        {
            MessageBox.Show("Не удалось открыть сундук");
        }
    }
    
}