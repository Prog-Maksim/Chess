using System.Text.Json.Serialization;

namespace Frontend.Models;

public class Token: BaseResponse
{

    public required string accessToken { get; set; }
}