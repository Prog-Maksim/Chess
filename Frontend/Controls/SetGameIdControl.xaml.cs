using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Frontend.Models;
using Frontend.Script;
using Frontend.Windows;

namespace Frontend.Controls;

public partial class SetGameIdControl : UserControl
{
    private readonly MainMenu.CloseMenu _closeMenu;
    
    public SetGameIdControl(MainMenu.CloseMenu closeMenu)
    {
        InitializeComponent();
        
        _closeMenu = closeMenu;

        var client = WebSocketService.Instance;
        Click.IsEnabled = client.IsConnected();
    }
    
    /// <summary>
    /// Отправляет запрос на вступление в игру
    /// </summary>
    /// <param name="gameId"></param>
    private async Task SendRequestInGame(string gameId)
    {
        HttpClient client = new HttpClient();
        
        var url = Url.BaseUrl + $"Game/login-game?gameId={gameId}";
        
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {SaveRepository.LoadTokenFromFile().AccessToken}");
        
        var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(url, content);
        
        string responseContent = await response.Content.ReadAsStringAsync();
        BaseResponse? result = JsonSerializer.Deserialize<BaseResponse>(responseContent);
        if (result != null)
        {
            SetTextError(result.message, !result.success);
            Click.Content = "Ожидание ответа...";
            
            if (!result.success)
                SetButtonEnabled();
        }
    }

    private void Click_OnClick(object sender, RoutedEventArgs e)
    {
        if (GameIdTextBox.Text.Length > 0)
        {
            Click.IsEnabled = false;
            Click.Content = "Загрузка...";
            _ = SendRequestInGame(GameIdTextBox.Text);
        }
        else
            SetTextError("Поле 'код игры' обязательно к заполнению", true);
    }

    public void SetButtonEnabled()
    {
        Click.Content = "Присоединиться";
        Click.IsEnabled = true;
        GameIdTextBox.Text = "";
    }
    
    private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _closeMenu();
    }

    public void SetTextError(string message, bool error = false)
    {
        InformText.Text = message;
        
        if(error)
            InformText.Foreground = Brushes.Red;
        else
            InformText.Foreground = Brushes.Black;
            
        ClearTextAfterDelay();
    }
    
    private async void ClearTextAfterDelay()
    {
        await Task.Delay(3000);
        InformText.Text = string.Empty;
    }
}