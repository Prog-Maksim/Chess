using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Frontend.Enums;
using Frontend.Models.WebSocketMessage;
using Frontend.Windows;

namespace Frontend.Controls.ResultGame;

public partial class GameTheResult : UserControl
{
    private readonly MainMenu _menu;
    
    public GameTheResult()
    {
        InitializeComponent();
    }

    public GameTheResult(bool isWinner, string leagueName, int score, int addScore, int unUsePotionScore, 
        int remainingTimeScore, int totalScore, int range, AddPotion? addPotion, List<PotionType> usedPotions, MainMenu menu) : this()
    {
        _menu = menu;
        
        if (isWinner)
        {
            MainText.Text = "ПОБЕДА";
            MainText.Foreground = (Brush)new BrushConverter().ConvertFrom("#57B300");
        }
        else
        {
            MainText.Text = "ПОРАЖЕНИЕ";
            MainText.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF0000");
        }
        
        LeagueName.Text = leagueName;
        ScoreText.Text = score.ToString();
        AddScoreText.Text = addScore.ToString();
        PotionScore.Text = unUsePotionScore.ToString();
        DurationScore.Text = remainingTimeScore.ToString();
        Total.Text = totalScore.ToString();

        if (range < 0)
        {
            Range.Foreground = Brushes.Red;
            Range.Text = range.ToString();
        }

        if (range == 0)
        {
            Range.Foreground = Brushes.Black;
            Range.Text = range.ToString();
        }
        else
            Range.Text = $"+{range}";

        if (addPotion != null)
        {
            ImagePotion.Source = GetPotionImage(addPotion.Type);
            PotionName.Text = addPotion.PotionName;
            Count.Text = addPotion.Count.ToString();
        }
        else
            MyGrid.RowDefinitions[2].Height = new GridLength(0);

        if (usedPotions != null || usedPotions.Count != 0)
        {
            for (int i = 0; i < usedPotions.Count; i++)
            {
                if (i == 0)
                    Potion1.Source = GetPotionImage(usedPotions[i]);
                else if (i == 1)
                    Potion2.Source = GetPotionImage(usedPotions[i]);
                else if (i == 2)
                    Potion3.Source = GetPotionImage(usedPotions[i]);
                else if (i == 3)
                    Potion4.Source = GetPotionImage(usedPotions[i]);
                else if (i == 4)
                    Potion5.Source = GetPotionImage(usedPotions[i]);
            }
        }
        else
            MyGrid.RowDefinitions[3].Height = new GridLength(0);
    }

    private BitmapImage GetPotionImage(PotionType type)
    {
        string basePath = AppDomain.CurrentDomain.BaseDirectory;
        string fullPath = Path.Combine(basePath, "Image", "Potion", $"{type}.png");

        return new BitmapImage(new Uri(fullPath, UriKind.Absolute));
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        _menu.OpenMainMenu();
    }
}