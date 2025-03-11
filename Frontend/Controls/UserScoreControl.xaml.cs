using System.Windows.Controls;

namespace Frontend.Controls;

public partial class UserScoreControl : UserControl
{
    public UserScoreControl()
    {
        InitializeComponent();
    }

    public void UpdateScore(int score)
    {
        ScoreText.Text = score.ToString();
    }
}