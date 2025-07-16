namespace DeepSeekChat.Core.Models;

public enum ProgressStatus
{
    InProgress,
    Completed,
    Stoped,
    LengthTerminated,
    Failed,
    None = -1
}

public partial class AiChatCompletionModel
{
    public string ReasoningContent { get; set; }

    public string Content { get; set; }
}

public partial class ApplicationChatMessageModel
{
    public string UserPrompt { get; set; }

    public AiChatCompletionModel AiChatCompletion { get; set; }

    public TokenUsageModel TokenUsage { get; set; }

    public ChatCompletionMetadataModel CurrentMessageMetadata { get; set; }

    public string Id { get; set; }

    public ProgressStatus ProgressStatus { get; set; }
}
