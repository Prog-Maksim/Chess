using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Frontend.Enums;

namespace Frontend.Controls.Potion;

public partial class UsePotionMenu : UserControl
{
    public UsePotionMenu()
    {
        InitializeComponent();
    }
    
    public UsePotionMenu(PotionType type, string playerName, string potionName): this()
    {
        ImagePotion.Source = GetPotionImage(type);
        PlayerName.Text = playerName;
        PotionName.Text = potionName;
    }
    
    private BitmapImage GetPotionImage(PotionType type)
    {
        string basePath = AppDomain.CurrentDomain.BaseDirectory;
        string fullPath = Path.Combine(basePath, "Image", "Potion", $"{type}.png");

        return new BitmapImage(new Uri(fullPath, UriKind.Absolute));
    }

}