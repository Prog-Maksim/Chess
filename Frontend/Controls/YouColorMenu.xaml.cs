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
        if (color == "#000000")
            TextBlock.Foreground = Brushes.White;
        MainBorder.Background = (Brush)new BrushConverter().ConvertFrom(color);
    }
}