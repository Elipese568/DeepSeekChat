namespace DeepSeekChat.Core.AI.Internal.Response;

public class TokenUsage
{
    public int PromptTokens { get; set; }
    public int CompletionTokens { get; set; }
    public int TotalTokens { get; set; }

    /// <summary>
    /// Special for DeepSeek Model response from SiliconFlow
    /// </summary>
    public CompletionTokenDetails CompletionTokenDetails { get; set; }
}
