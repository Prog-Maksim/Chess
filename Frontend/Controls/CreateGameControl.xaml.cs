﻿using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Frontend.Models;
using Frontend.Script;
using Frontend.Windows;

namespace Frontend.Controls;

public partial class CreateGameControl : UserControl
{
    private MainWindow _mainWindow;
    private MainMenu _mainMenu;
    private readonly MainMenu.CloseMenu _closeMenu;
    
    public CreateGameControl(MainWindow mainWindow, MainMenu mainMenu, MainMenu.CloseMenu closeMenu)
    {
        InitializeComponent();
        
        _mainWindow = mainWindow;
        _mainMenu = mainMenu;
        _closeMenu = closeMenu;
    }

    private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _closeMenu();
    }
    
    
    private async Task SendCreateGame(string name, int players = 2, bool isPrivate = false)
    {
        Click.Content = "загрузка...";
        Click.IsEnabled = false;
        
        HttpClient client = new HttpClient();
        var url = Url.BaseUrl + $"Game/create-game?name={name}&players={players}&isPrivate={isPrivate}";
        
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {SaveRepository.ReadToken()}");
        
        var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await client.PostAsync(url, content);
        
        if (response.IsSuccessStatusCode)
        {
            string responseBody = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<CreateGame>(responseBody);

            Console.WriteLine($"GameId: {data.gameId}");
            _mainWindow.OpenGameWindow(data.gameId, _mainMenu, true);
        }
        else
        {
            MessageBox.Show("Не удалось создать игру!");
        }
        
        Click.Content = "Создать";
        Click.IsEnabled = true;
    }

    private void Click_OnClick(object sender, RoutedEventArgs e)
    {
        string name = NameGame.Text;

        if (name.Length == 0)
        {
            ErrorText.Text = "Поле `Название` обязательно к заполнению";
            RemoveWarningAfterDelay();
            return;
        }
        
        int players = ToggleButton.IsChecked == true ? 4 : 2;
        bool isPrivate = CheckBox.IsChecked?? false;
        
        _ = SendCreateGame(name, players, isPrivate);
    }
    
    private async void RemoveWarningAfterDelay(int time = 1500)
    {
        await Task.Delay(time);
        ErrorText.Text = "";
    }
}