using System.Windows.Controls;
using System.Windows.Media;

namespace Frontend.Controls;

public partial class YouColorMenu : UserControl
{
    public YouColorMenu()
    {
        InitializeComponent();
    }

    public YouColorMenu(string color): this()
    {
        if (color == "#eeeeee")
            TextBlock.Foreground = Brushes.Black;
        MainBorder.Background = (Brush)new BrushConverter().ConvertFrom(color);
    }

    public void UpdateColor(string color)
    {
        if (color == "#eeeeee")
            TextBlock.Foreground = Brushes.Black;
        MainBorder.Background = (Brush)new BrushConverter().ConvertFrom(color);
    }
}