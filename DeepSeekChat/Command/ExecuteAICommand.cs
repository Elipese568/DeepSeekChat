using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using DeepSeekChat.Models;
using DeepSeekChat.Service;
using Microsoft.UI.Dispatching;
using OpenAI;
using OpenAI.Chat;

namespace DeepSeekChat.Command;

public enum UpdateType
{
    Reasoning,
    Content
}

public record ChatResponseReceivedEventArgs(string ContentUpdate, UpdateType Type, TokenUsage TokenUsage);
public record ChatResponseCompletedEventArgs(ProgressStatus Status);

public record ChatCompletionMetadata(string Id, DateTime TimeCreated, string Model, ChatOptions Options);

public class NoClientException : Exception
{
    public NoClientException(string message) : base(message)
    {
    }
}

public class ExecuteAICommand : ICommand
{
    private const string DoneMarker = "data: [DONE]";

    private readonly DiscussionItem _discussItem;
    private readonly DispatcherQueue _dispatcherQueue;
    private readonly ClientService _clientService;

    private ChatClient _chatClient;
    private CancellationTokenSource _cts;
    private bool _isRunning;

    public event EventHandler CanExecuteChanged;
    public event EventHandler<ChatResponseReceivedEventArgs> StreamResponseReceived;
    public event EventHandler<ChatResponseCompletedEventArgs> StreamCompleted;
    public event EventHandler<ChatCompletionMetadata> CompletionMetadataReceived;

    public ExecuteAICommand(DiscussionItem discussItem)
    {
        _discussItem = discussItem ?? throw new ArgumentNullException(nameof(discussItem));
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        var settingService = App.Current.GetService<SettingService>();

        
        _clientService = App.Current.GetService<ClientService>();
        _chatClient = _clientService.GetChatClient();
        
        _clientService.ClientUpdate += (s, e) =>
        {
            _chatClient = e.ChatClient;
        };
    }

    public bool CanExecute(object? parameter) => !_isRunning && _chatClient != null;

    public async void Execute(object parameter)
    {
        try
        {
            IsRunning = true;
            using (_cts = new CancellationTokenSource())
            {
                var messages = BuildMessageThread();
                var options = CreateChatOptions(_discussItem.ChatOptions);
                bool isMetadataReported = false;
                CompletionMetadataReceived?.Invoke(this, new("No Received", DateTime.Now, _clientService.Model, _discussItem.ChatOptions));

                await foreach (var chunk in await _clientService.CompleteChatStreamingAsync(messages, options, _cts))
                {
                    if (!isMetadataReported)
                    {
                        CompletionMetadataReceived?.Invoke(this, new(chunk.Id, DateTimeOffset.FromUnixTimeSeconds(chunk.Created).LocalDateTime, _clientService.Model, _discussItem.ChatOptions));
                        isMetadataReported = true;
                    }

                    var choice = chunk.Choices[0];
                    if (choice.FinishReason != null && choice.FinishReason != "stop")
                    {
                        throw new InvalidOperationException($"Unexpected finish reason: {choice.FinishReason}");
                    }

                    var delta = choice.Delta;
                    if (!string.IsNullOrEmpty(delta.ReasoningContent))
                    {
                        RaiseStreamEvent(delta.ReasoningContent, UpdateType.Reasoning, chunk.Usage);
                    }
                    else if (!string.IsNullOrEmpty(delta.Content))
                    {
                        RaiseStreamEvent(delta.Content, UpdateType.Content, chunk.Usage);
                    }
                }

                NotifyCompletion(ProgressStatus.Completed);
                IsRunning = false;
            }
        }
        catch (OperationCanceledException)
        {
            NotifyCompletion(ProgressStatus.Stoped);
            IsRunning = false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Chat processing failed: {ex}");
            NotifyCompletion(ProgressStatus.Failed);
            IsRunning = false;
        }
    }

    private List<ChatMessage> BuildMessageThread()
    {
        var messages = new List<ChatMessage>();
        messages.Add(SystemChatMessage.CreateSystemMessage(_discussItem.ChatOptions.SystemPrompt));

        foreach (var msg in _discussItem.Messages)
        {
            if (!string.IsNullOrEmpty(msg.UserPrompt))
            {
                messages.Add(UserChatMessage.CreateUserMessage(msg.UserPrompt));
            }
            if (!string.IsNullOrEmpty(msg.AiChatCompletion.Content))
            {
                messages.Add(AssistantChatMessage.CreateAssistantMessage(msg.AiChatCompletion.Content));
            }
        }
        return messages;
    }

    private static ChatCompletionOptions CreateChatOptions(ChatOptions options) => new()
    {
        MaxOutputTokenCount = options.MaxTokens,
        Temperature = options.Temperature,
        TopP = options.TopP,
        FrequencyPenalty = options.FrequencyPenalty,
        Seed = options.Seed
    };

    private void RaiseStreamEvent(string content, UpdateType type, TokenUsage usage)
    {
        StreamResponseReceived?.Invoke(this, new(content, type, usage));
    }

    private void NotifyCompletion(ProgressStatus status)
    {
        _dispatcherQueue.TryEnqueue(() =>
            StreamCompleted?.Invoke(this, new ChatResponseCompletedEventArgs(status)));
    }

    public void Cancel() => _cts?.Cancel();

    public bool IsRunning
    {
        get => _isRunning;
        set
        {
            if (_isRunning == value) return;

            _isRunning = value;
            _dispatcherQueue.TryEnqueue(() =>
                CanExecuteChanged?.Invoke(this, EventArgs.Empty));
        }
    }
}
