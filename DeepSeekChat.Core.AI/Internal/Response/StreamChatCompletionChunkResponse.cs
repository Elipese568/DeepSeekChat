using DeepSeekChat.Core.Network.Body;
using DeepSeekChat.Core.Network.Body.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.Core.AI.Internal.Response;

[ResponsePropertyNamingPolicy(NamingPolicyMode = ResponsePropertyNamingMode.SnakeCaseLower)]
public class StreamChatCompletionChunkResponse : ResponseBody
{
    public string Id { get; set; }
    public int Created { get; set; }
    public string Model { get; set; }
    public string Object { get; set; }
    public TokenUsage Usage { get; set; }
    public List<StreamChatCompletionChoice> Choices { get; set; }
}