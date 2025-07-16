using System;
using System.Text.Json.Serialization;

namespace DeepSeekChat.Core.Network.Model.Errors;

public class OpenIdentityDocumentDictError : ResponseError
{
    [JsonPropertyName("error")]
    public string Name { get; set; }

    [JsonPropertyName("error_description")]
    public string Description { get; set; }

    [JsonPropertyName("error_uri")]
    public Uri Uri { get; set; }
}
