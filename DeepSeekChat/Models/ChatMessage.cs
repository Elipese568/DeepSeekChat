using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenAI;
using OpenAI.Chat;
using DeepSeekChat.Helper.Converters;
using DeepSeekChat.Helper;
using Microsoft.UI.Xaml.Media;
using System.Text.Json.Serialization;
using DeepSeekChat.Command;

namespace DeepSeekChat.Models;

public enum ProgressStatus
{
    InProgress,
    Completed,
    Stoped,
    TooLongExited,
    Failed,
    None = -1
}

public partial class AiChatCompletion : ObservableObject
{
    [ObservableProperty]
    private string _reasoningContent;

    [ObservableProperty]
    private string _content;
}

public partial class ApplicationChatMessage : ObservableObject
{
    [ObservableProperty]
    private string _userPrompt;

    [ObservableProperty]
    private AiChatCompletion _aiChatCompletion;

    [ObservableProperty]
    private TokenUsage _tokenUsage;

    [ObservableProperty]
    private ChatCompletionMetadata _currentMessageMetadata;

    [ObservableProperty]
    private string _id;

    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    [ObservableProperty]
    private ProgressStatus _progressStatus;
}
