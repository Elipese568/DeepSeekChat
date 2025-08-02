using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace DeepSeekChat.Models;

public class ChatOptions
{
    public string SystemPrompt { get; set; } = "现在你是一个名为\"Deepseek R1\"的通用大语言推理模型，请使用 Markdown 格式（段落之间，大代码块头尾（“```language”，“```”）与代码之间，列表项之间，分点项（- content -content 或 1.content 2.content等）之间需要换行）回答用户的 Prompt";

    public float Temperature { get; set; } = 0.6f;

    public int MaxTokens { get; set; } = 8192;

    public float TopP { get; set; } = 0.95f;

    public float FrequencyPenalty { get; set; } = 0.0f;

    public int Seed { get; set; } = Random.Shared.Next();

    public bool SeedAutoRefresh { get; set; } = true;
    public bool StreamingOutput { get; set; } = true;
    public bool DetailedRequest { get; set; } = false;
}