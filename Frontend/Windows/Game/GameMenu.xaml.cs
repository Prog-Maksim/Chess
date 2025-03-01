using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Frontend.Controls;
using Frontend.Models.WebSockerMessage;
using Frontend.Scrypt;

namespace Frontend.Windows.Game;

public partial class GameMenu : Page
{
    private string _gameId;
    private Dictionary<string, PlayerTimeMenu> _players;
    
    public GameMenu()
    {
        InitializeComponent();
        GenerateChessBoard();
    }
    
    public GameMenu(string gameId, WebSocketService webSocket): this()
    {
        gameId = gameId;
        _players = new Dictionary<string, PlayerTimeMenu>();
        
        webSocket.OnDurationGame += UpdateGameTime;
        webSocket.OnAddPerson += WebSocketOnOnAddPerson; 
        webSocket.OnRemainingTimePerson += WebSocketOnOnRemainingTimePerson;
    }

    private void WebSocketOnOnRemainingTimePerson(object? sender, RemainingTimePerson e)
    {
        if (_players.ContainsKey(e.PersonId))
            _players[e.PersonId].UpdateTime(e.Time);
        else
        {
            PlayerTimeMenu playerTimeMenu = new PlayerTimeMenu(e.PersonId, new TimeSpan(hours: 0, minutes: 25, seconds: 0));
            _players[e.PersonId] = playerTimeMenu;
            StackPanelPlayer.Children.Add(playerTimeMenu);
        }
    }

    private void WebSocketOnOnAddPerson(object? sender, AddPerson e)
    {
        PlayerTimeMenu playerTimeMenu = new PlayerTimeMenu(e.Nickname, new TimeSpan(hours: 0, minutes: 25, seconds: 0));
        _players[e.PersonId] = playerTimeMenu;
        StackPanelPlayer.Children.Add(playerTimeMenu);
    }

    private void GenerateChessBoard()
    {
        int size = 8;
        
        ChessBoardGrid.Children.Clear();
        
        Brush lightColor = (Brush)new BrushConverter().ConvertFrom("#E0E0E0");
        Brush darkColor = (Brush)new BrushConverter().ConvertFrom("#BEBEBE");
        
        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < size; col++)
            {
                Border cell = new Border
                {
                    Background = (row + col) % 2 == 0 ? lightColor : darkColor,
                    CornerRadius = GetCornerRadius(row, col)
                };
                
                
                Grid.SetRow(cell, row);
                Grid.SetColumn(cell, col);
                ChessBoardGrid.Children.Add(cell);
            }
        }
    }
    
    private CornerRadius GetCornerRadius(int row, int col)
    {
        int radius = 5;
        
        if (row == 0 && col == 0)
            return new CornerRadius(radius, 0, 0, 0);
        if (row == 7 && col == 0)
            return new CornerRadius(0, 0, 0, radius);
        if (row == 0 && col == 7)
            return new CornerRadius(0, radius, 0, 0);
        if (row == 7 && col == 7)
            return new CornerRadius(0, 0, radius, 0);
    
        return new CornerRadius(0);
    }

    private void UpdateGameTime(object? o, DurationGame game)
    {
        GameTime.Text = game.Time.ToString();
    }
}