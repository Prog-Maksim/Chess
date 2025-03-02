namespace Frontend.Models;

public class Token: BaseResponse
{

    public required string accessToken { get; set; }
    public required string personId { get; set; }
}