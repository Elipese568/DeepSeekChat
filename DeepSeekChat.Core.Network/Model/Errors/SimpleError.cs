using System.Text.Json.Serialization;

namespace DeepSeekChat.Core.Network.Model.Errors;

public class SimpleError : ResponseError
{
    [JsonPropertyName("detail")]
    public string Detail { get; set; }

    [JsonPropertyName("instance")]
    public string Instance { get; set; }

    [JsonPropertyName("status")]
    public long? Status { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }
}
