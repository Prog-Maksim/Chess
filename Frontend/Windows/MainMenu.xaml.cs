using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Frontend.Controls;
using Frontend.Controls.Message;
using Frontend.Models.WebSocketMessage;
using Frontend.Script;

namespace Frontend.Windows;

public partial class MainMenu : Page
{
    public delegate void GetGameId(string gameId);

    private readonly WebSocketService client;
    
    private MainWindow mainWindow;
    
    public MainMenu()
    {
        InitializeComponent();
        
        client = WebSocketService.Instance;
        client.OnIsConnected += ClientOnIsConnected;
        client.OnConnectRetry += ClientOnConnectRetry;
        client.OnFailedConnect += ClientOnFailedConnect;
    }

    public delegate void RetryConnect();
    
    private void ClientOnFailedConnect(object? sender, EventArgs e)
    {
        RetryConnect deleteNotify = () =>
        {
            RemoveWarningAfterDelay(0);
            _ = client.ConnectAsync(SaveRepository.ReadToken());
        };
        FailedMessageConnect warningRetryConnect = new FailedMessageConnect(deleteNotify);
        warningRetryConnect.Margin = new Thickness(0, 10, 0, 35);
        Notify.Child = warningRetryConnect;
    }

    private void ClientOnConnectRetry(object? sender, EventArgs e)
    {
        RemoveWarningAfterDelay(0);
        
        WarningRetryConnect warningRetryConnect = new WarningRetryConnect();
        warningRetryConnect.Margin = new Thickness(0, 10, 0, 35);
        Notify.Child = warningRetryConnect;
        
        RemoveWarningAfterDelay();
    }
    
    private void ClientOnIsConnected(object? sender, bool e)
    {
        CoopButton.IsEnabled = e;
        CreateGame.IsEnabled = e;
        JoinTheGame.IsEnabled = e;
    }
    
    private async void RemoveWarningAfterDelay(int time = 1500)
    {
        await Task.Delay(time);
        Notify.Child = null;
    }

    public MainMenu(WebSocketService webSocket, MainWindow mainWindow): this()
    {
        this.mainWindow = mainWindow;
        webSocket.OnResultJoinTheGame += StartGame;
    }

    public delegate void CloseMenu();
    
    private CreateGameControl? CreateGameControl;
    private void CreateGame_OnClick(object sender, RoutedEventArgs e)
    {
        if (CreateGameControl != null)
            return;
        
        CloseMenu closeMenu = () =>
        {
            MainGrid.Children.Remove(CreateGameControl);
            SetGameIdControl = null;
            CoopButton_OnClick(sender, e);
        };
        CreateGameControl = new CreateGameControl(mainWindow, this, closeMenu);
        
        Grid.SetRowSpan(CreateGameControl, 4);
        Grid.SetColumnSpan(CreateGameControl, 4);
        Panel.SetZIndex(CreateGameControl, 2);
        MainGrid.Children.Add(CreateGameControl);
    }
    
    
    private SetGameIdControl? SetGameIdControl;
    private void JoinTheGame_OnClick(object sender, RoutedEventArgs e)
    {
        if (SetGameIdControl != null)
            return;
        
        CloseMenu closeMenu = () =>
        {
            MainGrid.Children.Remove(SetGameIdControl);
            SetGameIdControl = null;
            CoopButton_OnClick(sender, e);
        };
        SetGameIdControl = new SetGameIdControl(closeMenu);
        
        Grid.SetRowSpan(SetGameIdControl, 4);
        Grid.SetColumnSpan(SetGameIdControl, 4);
        Panel.SetZIndex(SetGameIdControl, 2);
        MainGrid.Children.Add(SetGameIdControl);
    }
    
    private void StartGame(object? sender, ResultJoinTheGame result)
    {
        if (result.Status)
            mainWindow.OpenGameWindow(result.GameId, this);
        else
        {
            if (SetGameIdControl != null)
                SetGameIdControl.SetTextError("Вам запретили войти в игру", true);
        }
        
        if (SetGameIdControl != null)
            SetGameIdControl.SetButtonEnabled();
    }

    public void OpenMainMenu()
    {
        mainWindow.OpenMainWindow();
    }

    private bool _buttonIsOpen;
    private void CoopButton_OnClick(object sender, RoutedEventArgs e)
    {
        CoopPopup.IsOpen = !_buttonIsOpen;
        _buttonIsOpen = !_buttonIsOpen;
        
        if (_buttonIsOpen)
            CoopButton.Background = (Brush)new BrushConverter().ConvertFrom("#5053A7");
        else
            CoopButton.Background = (Brush)new BrushConverter().ConvertFrom("#7074D5");
    }
}