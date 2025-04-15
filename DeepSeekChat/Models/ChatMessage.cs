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
    public string ReasoningContent { get; set; }

    public string Content { get; set; }
}

public partial class ApplicationChatMessage : ObservableObject
{
    public string UserPrompt { get; set; }

    public AiChatCompletion AiChatCompletion { get; set; }

    public TokenUsage TokenUsage { get; set; }

    public ChatCompletionMetadata CurrentMessageMetadata { get; set; }

    public string Id { get; set; }

    public ProgressStatus ProgressStatus { get; set; }
}
