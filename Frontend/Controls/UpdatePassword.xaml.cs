using System.Windows;
using System.Windows.Controls;

namespace Frontend.Controls;

public partial class UpdatePassword : UserControl
{
    private readonly Settings.Passwd _update;
    
    public UpdatePassword(Settings.Passwd update)
    {
        InitializeComponent();
        _update = update;
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        _update(OldPassword.Text, NewPassword.Text);
    }
}