using DeepSeekChat.Core.AI.Chat.Primitives;
using DeepSeekChat.Core.AI.Internal.Request;
using DeepSeekChat.Core.Network.Body;
using DeepSeekChat.Core.Network.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.Core.AI.Chat.Message;

public abstract class ChatMessage : IRequestBodyConvertable<MessageRequestBody>
{
    /// <summary>
    /// The role of the message sender.
    /// </summary>
    public abstract Role Role { get; }
    /// <summary>
    /// The content of the message.
    /// </summary>
    public abstract string Content { get; set; }

    public MessageRequestBody ToRequestBody()
    {
        return new MessageRequestBody()
        {
            Content = Content,
            Role = Role switch
            {
                Role.User => "user",
                Role.Assistant => "assistant",
                Role.System => "system",
                Role.Tool => "tool",
                _ => throw new NotSupportedException("What's going on?")
            }
        };
    }
}
