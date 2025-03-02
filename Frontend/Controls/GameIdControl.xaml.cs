using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Frontend.Controls;

public partial class GameIdControl : UserControl
{
    public GameIdControl()
    {
        InitializeComponent();
    }

    public GameIdControl(string gameId) : this()
    {
        GameIdText.Text = gameId;
    }

    private void CopyIcon_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        Clipboard.SetText(GameIdText.Text);
    }
}