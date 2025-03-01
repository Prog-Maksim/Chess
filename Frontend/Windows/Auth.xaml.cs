using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Frontend.Models;
using Frontend.Script;

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

        _nickname = NicknameTextBox;
        _email = EmailTextBox;
        _password = PasswordTextBox;
    }

    public Auth(MainWindow.OpenMainWindowDelegate  func): this()
    {
        this.func = func;
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
            
            _email = new TextBox();
            _email.Tag = "Почта";
            _email.Style = FindResource("TextBoxStyle") as Style;
            
            _password = new PasswordBox();
            _password.Tag = "Пароль";
            _password.Margin = new Thickness(0, 0, 0, 10);
            _password.Style = FindResource("PasswordBoxStyle") as Style;
            
            TextBoxList.Children.Add(_email);
            TextBoxList.Children.Add(_password);
        }
        else
        {
            state = "registration";
            
            MainTextBlock.Text = "Регистрация";
            AuthButton.Content = "Зарегистрироваться";
            ChoiseText.Text = "уже есть аккаунт?";
            
            TextBoxList.Children.Clear();
            
            _nickname = new TextBox();
            _nickname.Tag = "Никнейм";
            _nickname.Style = FindResource("TextBoxStyle") as Style;
            
            _email = new TextBox();
            _email.Tag = "Почта";
            _email.Style = FindResource("TextBoxStyle") as Style;
            
            _password = new PasswordBox();
            _password.Tag = "Пароль";
            _password.Margin = new Thickness(0, 0, 0, 10);
            _password.Style = FindResource("PasswordBoxStyle") as Style;
            
            TextBoxList.Children.Add(_nickname);
            TextBoxList.Children.Add(_email);
            TextBoxList.Children.Add(_password);
        }
    }

    private void AuthButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (state == "registration")
            _ = Registration(_nickname.Text, _email.Text, _password.Password);
        else
            _ = Authorization(_email.Text, _password.Password);
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