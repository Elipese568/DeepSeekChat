using DeepSeekChat.Core.AI.Chat.Primitives;
using System;

namespace DeepSeekChat.Core.AI.Chat.Message;

public class SystemMessage : ChatMessage
{
    public override Role Role => Role.System;
    public override string Content { get; set; }
    public SystemMessage(string content)
    {
        Content = content ?? throw new ArgumentNullException(nameof(content));
    }
}
