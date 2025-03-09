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
        try
        {
            Clipboard.SetText(GameIdText.Text);
        }
        catch
        {
            MessageBox.Show("Не удалось скопировать id карты \n\nВозможно объект буфера обмена занят другим процессом\nПовторите попытку еще раз", "Chess-online", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}