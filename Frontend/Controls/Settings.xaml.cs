using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using Frontend.Script;
using Frontend.Windows;

namespace Frontend.Controls;

public partial class Settings : UserControl
{
    private MainMenu.CloseMenu _closeMenu;
    
    public Settings()
    {
        InitializeComponent();
    }
    
    public Settings(MainMenu.CloseMenu closeMenu) : this()
    {
        _closeMenu = closeMenu;
    }

    private void Exit_OnClick(object sender, RoutedEventArgs e)
    {
        SaveRepository.DeleteFile();

        Application.Current.Shutdown();
        Process.Start(Path.GetFullPath(Process.GetCurrentProcess().MainModule.FileName));
    }

    private void DeleteAccount_OnClick(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show("Вы уверены что хотите удалить аккаунт без возможности восстановления?", "Chess-online", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
            SendRequestDeleteAccount();
    }

    private async void SendRequestDeleteAccount()
    {
        using HttpClient client = new HttpClient();
        var url = Url.BaseUrl + "Profile/account";

        using var request = new HttpRequestMessage(HttpMethod.Delete, url);
        request.Headers.Add("Authorization", $"Bearer {SaveRepository.LoadTokenFromFile().AccessToken}");
        request.Headers.Add("Accept", "application/json");

        using var response = await client.SendAsync(request);

        if (response.StatusCode != HttpStatusCode.OK)
            MessageBox.Show("Не удалось удалить аккаунт!");
        
        SaveRepository.DeleteFile();
        
        Application.Current.Shutdown();
        Process.Start(Path.GetFullPath(Process.GetCurrentProcess().MainModule.FileName));
    }

    public delegate void Passwd(string pass1, string pass2);
    private UpdatePassword? _updatePassword;
    private void UpdatePassword_OnClick(object sender, RoutedEventArgs e)
    {
        if (_updatePassword == null)
        {
            Passwd passwd = SendRequestUpdatePassword;
            _updatePassword = new UpdatePassword(passwd);
        }
        else
        {
            _updatePassword = null;
        }
    }

    private async void SendRequestUpdatePassword(string oldPassword, string newPassword)
    {
        using HttpClient client = new HttpClient();
        
        string url = Url.BaseUrl + "Auth/password";
        
        try
        {
            var requestBody = new
            {
                oldPassword = oldPassword,
                newPassword = newPassword
            };
            
            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var request = new HttpRequestMessage(HttpMethod.Put, url);
            request.Headers.Add("Authorization", $"Bearer {SaveRepository.LoadTokenFromFile().AccessToken}");
            request.Headers.Add("Accept", "application/json");
            request.Content = content;

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Request failed: {ex.Message}");
        }
    }

    private void Close_OnClick(object sender, RoutedEventArgs e)
    {
        _closeMenu();
    }
    
    
}