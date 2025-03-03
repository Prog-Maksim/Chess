using System.Windows.Controls;
using System.Windows.Media;

namespace Frontend.Controls;

public partial class PlayerTimeMenu : UserControl
{
    public PlayerTimeMenu()
    {
        InitializeComponent();
    }

    public PlayerTimeMenu(string nickname, TimeSpan ts): this()
    {
        NameTextBox.Text = nickname;
        TimeTextBox.Text = $"{ts.Minutes:D2}:{ts.Seconds:D2}";
    }

    public void UpdateTime(TimeSpan ts)
    {
        TimeTextBox.Text = $"{ts.Minutes:D2}:{ts.Seconds:D2}";
    }

    public void IsTern(bool isYou = false)
    {
        if (isYou)
            MainBorder.Background = (Brush)new BrushConverter().ConvertFrom("#7DFF67");
        else 
            MainBorder.Background = (Brush)new BrushConverter().ConvertFrom("#CFCFCF");
    }

    public void EndTern()
    {
        MainBorder.Background = (Brush)new BrushConverter().ConvertFrom("#E0E0E0");
    }

    public void IsGameOver()
    {
        MainBorder.Background = Brushes.Red;
    }
}