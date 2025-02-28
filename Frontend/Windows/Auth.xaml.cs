using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Frontend.Models;
using Frontend.Scrypt;

namespace Frontend.Windows;

public partial class Auth : Page
{
    private string state = "registration";

    private TextBox? Nickname;
    private TextBox Email;
    private PasswordBox Password;
    
    public Auth()
    {
        InitializeComponent();
    }

    private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (state == "registration")
        {
            state = "authorization";
            
            MainTextBlock.Text = "Авторизация";
            AuthButton.Content = "Авторизоваться";
            ChoiseText.Text = "еще нет аккаунта?";
            
            TextBoxList.Children.Clear();
            
            Email = new TextBox();
            Email.Tag = "Почта";
            Email.Style = FindResource("TextBoxStyle") as Style;
            
            Password = new PasswordBox();
            Password.Tag = "Пароль";
            Password.Margin = new Thickness(0, 0, 0, 10);
            Password.Style = FindResource("PasswordBoxStyle") as Style;
            
            TextBoxList.Children.Add(Email);
            TextBoxList.Children.Add(Password);
        }
        else
        {
            state = "registration";
            
            MainTextBlock.Text = "Регистрация";
            AuthButton.Content = "Зарегистрироваться";
            ChoiseText.Text = "уже есть аккаунт?";
            
            TextBoxList.Children.Clear();
            
            Nickname = new TextBox();
            Nickname.Tag = "Никнейм";
            Nickname.Style = FindResource("TextBoxStyle") as Style;
            
            Email = new TextBox();
            Email.Tag = "Почта";
            Email.Style = FindResource("TextBoxStyle") as Style;
            
            Password = new PasswordBox();
            Password.Tag = "Пароль";
            Password.Margin = new Thickness(0, 0, 0, 10);
            Password.Style = FindResource("PasswordBoxStyle") as Style;
            
            TextBoxList.Children.Add(Nickname);
            TextBoxList.Children.Add(Email);
            TextBoxList.Children.Add(Password);
        }
    }

    private void AuthButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (state == "registration")
            _ = Registration(Nickname.Text, Email.Text, Password.Password);
        else
            _ = Authorization(Email.Text, Password.Password);
    }

    private async Task Registration(string nickname, string email, string password)
    {
        using HttpClient client = new HttpClient();
        
        string url = Url.BaseUrl + "Auth/registration";
        var requestData = new { nickname, email, password };
        
        string json = JsonSerializer.Serialize(requestData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            HttpResponseMessage response = await client.PostAsync(url, content);
            string responseContent = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Token? token = JsonSerializer.Deserialize<Token>(responseContent);
            
                if (!token.success)
                    ErrorTextBlock.Text = token.message;
                SaveRepository.SaveToken(token.accessToken);
            }
            else
                ErrorTextBlock.Text = responseContent;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error");
        }
    }

    private async Task Authorization(string email, string password)
    {
        using HttpClient client = new HttpClient();
        
        string url = Url.BaseUrl + "Auth/authorization";
        var requestData = new { email, password };
        
        string json = JsonSerializer.Serialize(requestData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            HttpResponseMessage response = await client.PostAsync(url, content);
            string responseContent = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Token? token = JsonSerializer.Deserialize<Token>(responseContent);

                if (!token.success)
                    ErrorTextBlock.Text = token.message;
                SaveRepository.SaveToken(token.accessToken);
            }
            else
                ErrorTextBlock.Text = responseContent;

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error");
        }
    }
}