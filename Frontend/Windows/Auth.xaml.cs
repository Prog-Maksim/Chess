using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Frontend.Interfaces;
using Frontend.Models;
using Frontend.Script;
using PasswordBox = Frontend.Controls.PasswordBox;
using TextBox = Frontend.Controls.TextBox;

namespace Frontend.Windows;

public partial class Auth : Page
{
    private string state = "registration";
    private MainWindow.OpenMainWindowDelegate? func;

    private TextBox _nickname;
    private TextBox _email;
    private PasswordBox _password;
    
    public Auth()
    {
        InitializeComponent();
        AuthState();
    }

    public Auth(MainWindow.OpenMainWindowDelegate  func): this()
    {
        this.func = func;
    }

    private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (state == "registration")
            AuthState();
        else
            RegState();
    }

    private void AuthState()
    {
        state = "authorization";
            
        MainTextBlock.Text = "Авторизация";
        AuthButton.Content = "Авторизоваться";
        ChoiseText.Text = "еще нет аккаунта?";
            
        TextBoxList.Children.Clear();
            
        _email = new TextBox();
        _email.PreviewText = "Почта";
        _email.Margin = new Thickness(0, 0, 0, 30);
            
        _password = new PasswordBox();
        _password.PreviewText = "Пароль";
        _password.Margin = new Thickness(0, 0, 0, 10);
            
        TextBoxList.Children.Add(_email);
        TextBoxList.Children.Add(_password);
    }

    private void RegState()
    {
        state = "registration";
            
        MainTextBlock.Text = "Регистрация";
        AuthButton.Content = "Зарегистрироваться";
        ChoiseText.Text = "уже есть аккаунт?";
            
        TextBoxList.Children.Clear();
            
        _nickname = new TextBox();
        _nickname.PreviewText = "Никнейм";
        _nickname.Margin = new Thickness(0, 0, 0, 30);
            
        _email = new TextBox();
        _email.PreviewText = "Почта";
        _email.Margin = new Thickness(0, 0, 0, 30);
            
        _password = new PasswordBox();
        _password.PreviewText = "Пароль";
        _password.Margin = new Thickness(0, 0, 0, 10);
            
        TextBoxList.Children.Add(_nickname);
        TextBoxList.Children.Add(_email);
        TextBoxList.Children.Add(_password);
    }

    private void AuthButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (state == "registration")
            _ = Registration(_nickname.GetText(), _email.GetText(), _password.GetText());
        else
            _ = Authorization(_email.GetText(), _password.GetText());
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
                SaveRepository.SaveId(token.personId);
                
                if (func != null)
                    func();
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
                SaveRepository.SaveId(token.personId);
                
                if (func != null)
                    func();
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