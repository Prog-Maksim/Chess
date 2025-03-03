using System.Windows.Controls;
using Frontend.Interfaces;

namespace Frontend.Controls;

public partial class PasswordBox : UserControl, ITextBoxBase
{
    public PasswordBox()
    {
        InitializeComponent();
        
        this.DataContext = this;
    }
    
    public string PreviewText {get; set;}
    public string GetText()
    {
        return PasswordBoxBase.Password;
    }
}