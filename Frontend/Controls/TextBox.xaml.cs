﻿using System.Windows.Controls;
using Frontend.Interfaces;

namespace Frontend.Controls;

public partial class TextBox : UserControl, ITextBoxBase
{
    public TextBox()
    {
        InitializeComponent();
        
        this.DataContext = this;
    }
    
    public string PreviewText {get; set;}

    public string GetText()
    {
        return BaseTextBox.Text;
    }
}