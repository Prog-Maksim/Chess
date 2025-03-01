using System.Windows.Controls;

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
        TimeTextBox.Text = $"{ts.Minutes}:{ts.Seconds}";
    }

    public void UpdateTime(TimeSpan ts)
    {
        TimeTextBox.Text = $"{ts.Minutes}:{ts.Seconds}";
    }
}