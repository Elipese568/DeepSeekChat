using DeepSeekChat.Core.AI.Chat.Primitives;
using System;

namespace DeepSeekChat.Core.AI.Chat.Message;

public class AssistantMessage : ChatMessage
{
    public override Role Role => Role.Assistant;
    public override string Content { get; set; }
    public AssistantMessage(string content)
    {
        Content = content ?? throw new ArgumentNullException(nameof(content));
    }
}
