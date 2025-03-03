using System.Windows.Controls;
using System.Windows.Input;
using Frontend.Script;
using Frontend.Windows;

namespace Frontend.Controls.Message;

public partial class FailedMessageConnect : UserControl, INotify
{
    private readonly MainMenu.RetryConnect _deleteNotify;
    
    public FailedMessageConnect(MainMenu.RetryConnect deleteNotify)
    {
        InitializeComponent();
        _deleteNotify = deleteNotify;
    }

    private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _deleteNotify();
    }
}