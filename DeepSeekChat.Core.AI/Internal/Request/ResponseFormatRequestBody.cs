using DeepSeekChat.Core.Network.Body;
using DeepSeekChat.Core.Network.Body.Attributes;

namespace DeepSeekChat.Core.AI.Internal.Request;

public class ResponseFormatRequestBody : RequestBody
{
    public string Type { get; set; }
}
