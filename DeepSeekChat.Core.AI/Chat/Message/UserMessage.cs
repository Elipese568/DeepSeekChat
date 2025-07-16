using DeepSeekChat.Core.AI.Chat.Primitives;
using System;

namespace DeepSeekChat.Core.AI.Chat.Message;

public class UserMessage : ChatMessage
{
    public override Role Role => Role.User;
    public override string Content { get; set; }
    public UserMessage(string content)
    {
        Content = content ?? throw new ArgumentNullException(nameof(content));
    }
}
