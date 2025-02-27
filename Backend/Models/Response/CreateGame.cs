namespace Backend.Models.Response;

public class CreateGame: BaseResponse
{
    public required string GameId { get; set; }
}