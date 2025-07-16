using DeepSeekChat.Core.AI.Internal.Response.Exceptions;
using DeepSeekChat.Core.Network.Body;
using DeepSeekChat.Core.Network.Body.Attributes;
using System.Collections.Generic;

namespace DeepSeekChat.Core.AI.Internal.Response;

[ResponsePropertyNamingPolicy(NamingPolicyMode = ResponsePropertyNamingMode.SnakeCaseLower)]
[ResponseErrorTypes(400, typeof(GenericError400))]
[ResponseErrorTypes(401, typeof(string))]
[ResponseErrorTypes(404, typeof(string))]
[ResponseErrorTypes(429, typeof(RateLimitError429))]
[ResponseErrorTypes(503, typeof(ServiceOverloadError503))]
[ResponseErrorTypes(504, typeof(string))]
public class WholeChatCompletionResponse : ResponseBody
{
    public string Id { get; set; }
    public int Created { get; set; }
    public string Model { get; set; }
    public string Object { get; set; }
    public TokenUsage Usage { get; set; }
    public List<WholeChatCompletionChoice> Choices { get; set; }
}
