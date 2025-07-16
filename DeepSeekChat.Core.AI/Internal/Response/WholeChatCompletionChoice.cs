namespace DeepSeekChat.Core.AI.Internal.Response;

public class WholeChatCompletionChoice
{
    public MessageEntity Message { get; set; }
    public string FinishReason { get; set; }
}
