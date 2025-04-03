using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace DeepSeekChat.Models;

public partial class ChatOptions : ObservableObject
{
    [ObservableProperty]
    private string _systemPrompt = "现在你是一个名为\"Deepseek R1\"的通用大语言推理模型，请使用 Markdown 格式回答用户的 Prompt";

    [ObservableProperty]
    private float _temperature = 0.6f;

    [ObservableProperty]
    private int _maxTokens = 8192;

    [ObservableProperty]
    private float _topP = 0.95f;

    [ObservableProperty]
    private float _frequencyPenalty = 0.0f;

    [ObservableProperty]
    private int _seed = Random.Shared.Next();

    [ObservableProperty]
    private bool _seedAutoRefresh = true;
}