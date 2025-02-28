using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Frontend.Models.WebSockerMessage;
using Frontend.Scrypt;

namespace Frontend.Windows.Game;

public partial class GameMenu : Page
{
    public GameMenu()
    {
        InitializeComponent();
        GenerateChessBoard();
    }
    
    public GameMenu(WebSocketService webSocket): this()
    {
        webSocket.OnDurationGame += UpdateGameTime;
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
    
        return new CornerRadius(0); // Для остальных клеток без закругления
    }

    private void UpdateGameTime(object? o, DurationGame game)
    {
        GameTime.Text = game.Time.ToString();
    }
}