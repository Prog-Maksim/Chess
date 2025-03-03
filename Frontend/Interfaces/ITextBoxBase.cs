namespace Frontend.Interfaces;

public interface ITextBoxBase
{
    public string PreviewText { get; set; } 
    
    public string GetText();
}