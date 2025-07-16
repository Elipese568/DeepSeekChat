using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DeepSeekChat.Core.Models;

public class StreamingChatCompletionChunkModel
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("object")]
    public string ObjectType { get; set; }  // 避免C#关键字冲突

    [JsonPropertyName("created")]
    public long Created { get; set; }

    [JsonPropertyName("model")]
    public string Model { get; set; }

    [JsonPropertyName("choices")]
    public List<ChoiceModel> Choices { get; set; }

    //[JsonPropertyName("system_fingerprint")]
    //public string SystemFingerprint { get; set; }

    [JsonPropertyName("usage")]
    public TokenUsageModel Usage { get; set; }

    internal static StreamingChatCompletionChunkModel FromJson(string jsonString)
    {
        return JsonSerializer.Deserialize<StreamingChatCompletionChunkModel>(jsonString);
    }
}

public class ChoiceModel
{
    [JsonPropertyName("index")]
    public int Index { get; set; }

    [JsonPropertyName("delta")]
    public DeltaModel Delta { get; set; }

    [JsonPropertyName("finish_reason")]
    public string FinishReason { get; set; }
}

public class DeltaModel
{
    [JsonPropertyName("content")]
    public string Content { get; set; }

    [JsonPropertyName("reasoning_content")]
    public string ReasoningContent { get; set; }

    [JsonPropertyName("role")]
    public string Role { get; set; }
}

public class TokenUsageModel
{
    [JsonPropertyName("prompt_tokens")]
    public int PromptTokens { get; set; }

    [JsonPropertyName("completion_tokens")]
    public int CompletionTokens { get; set; }

    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; set; }

    internal static TokenUsageModel FromJson(string jsonString)
    {
        return JsonSerializer.Deserialize<TokenUsageModel>(jsonString);
    }
}
