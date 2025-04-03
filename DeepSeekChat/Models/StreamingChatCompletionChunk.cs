using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DeepSeekChat.Models;

public class StreamingChatCompletionChunk
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
    public List<Choice> Choices { get; set; }

    [JsonPropertyName("system_fingerprint")]
    public string SystemFingerprint { get; set; }

    [JsonPropertyName("usage")]
    public TokenUsage Usage { get; set; }

    internal static StreamingChatCompletionChunk? FromJson(string? jsonString)
    {
        return JsonSerializer.Deserialize<StreamingChatCompletionChunk>(jsonString);
    }
}

public class Choice
{
    [JsonPropertyName("index")]
    public int Index { get; set; }

    [JsonPropertyName("delta")]
    public Delta Delta { get; set; }

    [JsonPropertyName("finish_reason")]
    public string FinishReason { get; set; }
}

public class Delta
{
    [JsonPropertyName("content")]
    public string Content { get; set; }

    [JsonPropertyName("reasoning_content")]
    public string ReasoningContent { get; set; }

    [JsonPropertyName("role")]
    public string Role { get; set; }
}

public class TokenUsage
{
    [JsonPropertyName("prompt_tokens")]
    public int PromptTokens { get; set; }

    [JsonPropertyName("completion_tokens")]
    public int CompletionTokens { get; set; }

    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; set; }
}
