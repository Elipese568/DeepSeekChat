using DeepSeekChat.Core.Network.Body;
using DeepSeekChat.Core.Network.Body.Attributes;

namespace DeepSeekChat.Core.AI.Internal.Request;

public class MessageRequestBody : RequestBody
{
    public string Role { get; set; }
    public string Content { get; set; }
}
