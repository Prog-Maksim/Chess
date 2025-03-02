namespace Frontend.Models.Request;

public class MovePiece
{
    public string Type { get; set; }
    public string gameId { get; set; }
    public string token { get; set; }
    
    public int FromRow { get; set; }
    public int FromCol { get; set; }
    public int ToRow { get; set; }
    public int ToCol { get; set; }
}