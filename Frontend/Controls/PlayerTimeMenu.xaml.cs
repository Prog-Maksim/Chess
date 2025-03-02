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
        TimeTextBox.Text = $"{ts.Minutes:D2}:{ts.Seconds:D2}";
    }

    public void UpdateTime(TimeSpan ts)
    {
        TimeTextBox.Text = $"{ts.Minutes:D2}:{ts.Seconds:D2}";
    }
}