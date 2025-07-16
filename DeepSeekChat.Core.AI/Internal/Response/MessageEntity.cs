using System.Collections.Generic;

namespace DeepSeekChat.Core.AI.Internal.Response;

public class MessageEntity
{
    public string Role { get; set; }
    public string Content { get; set; }
    public List<ToolCall> ToolCalls { get; set; }
}
