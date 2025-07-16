using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.Core.AI.Chat.Primitives;

public enum Role
{
    /// <summary>
    /// The user who is interacting with the AI.
    /// </summary>
    User,
    /// <summary>
    /// The AI assistant responding to the user.
    /// </summary>
    Assistant,
    /// <summary>
    /// A system message that provides context or instructions.
    /// </summary>
    System,
    /// <summary>
    /// A message from a tool or external service.
    /// </summary>
    Tool
}
