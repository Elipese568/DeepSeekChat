namespace DeepSeekChat.Core.AI.Internal.Response;

public class StreamChatCompletionChoice
{
    public MessageEntity Message { get; set; }
    public string SystemFinger { get; set; }
    public string FinishReason { get; set; }
}
