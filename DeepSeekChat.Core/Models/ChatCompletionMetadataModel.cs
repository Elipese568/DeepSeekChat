using System;

namespace DeepSeekChat.Core.Models;

public class ChatCompletionMetadataModel
{
    public string Id { get; set; }
    public DateTime TimeCreated { get; set; }
    public string Model { get; set; }
    public ChatOptionsModel Options { get; set; }
}
