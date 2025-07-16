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
using DeepSeekChat.Service;
using Microsoft.UI.Dispatching;
using Microsoft.Extensions.AI;
using DeepSeekChat.Core.Models;

namespace DeepSeekChat.Command;

public enum UpdateType
{
    Reasoning,
    Content
}

public record ChatResponseReceivedEventArgs(string ContentUpdate, UpdateType Type, TokenUsageModel TokenUsage);
public record ChatResponseCompletedEventArgs(ProgressStatus Status);

public class NoClientException : Exception
{
    public NoClientException(string message) : base(message)
    {
    }
}

public class ExecuteAICommand : ICommand
{
    private const string DoneMarker = "data: [DONE]";

    private readonly DiscussionItemModel _discussItem;
    private readonly DispatcherQueue _dispatcherQueue;
    private readonly ClientService _clientService;

    private CancellationTokenSource _cts;
    private bool _isRunning;

    public event EventHandler CanExecuteChanged;
    public event EventHandler<ChatResponseReceivedEventArgs> StreamResponseReceived;
    public event EventHandler<ChatResponseCompletedEventArgs> StreamCompleted;
    public event EventHandler<ChatCompletionMetadataModel> CompletionMetadataReceived;

    public ExecuteAICommand(DiscussionItemModel discussItem)
    {
        _discussItem = discussItem ?? throw new ArgumentNullException(nameof(discussItem));
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        var settingService = App.Current.GetService<SettingService>();

        
        _clientService = App.Current.GetService<ClientService>();
    }

    public bool CanExecute(object? parameter) => !_isRunning && _clientService.GetChatClient() != null;

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
                CompletionMetadataReceived?.Invoke(this, new()
                {
                    Id = "No Received",
                    TimeCreated = DateTime.Now,
                    Model = _clientService.Model,
                    Options = _discussItem.ChatOptions
                });

                await foreach (var chunk in await _clientService.CompleteChatStreamingAsync(messages, options, _cts))
                {
                    if (!isMetadataReported)
                    {
                        CompletionMetadataReceived?.Invoke(this, new()
                        {
                            Id = chunk.Id,
                            TimeCreated = DateTimeOffset.FromUnixTimeSeconds(chunk.Created).LocalDateTime,
                            Model = _clientService.Model,
                            Options = _discussItem.ChatOptions
                        });
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
        messages.Add(new(ChatRole.System, _discussItem.ChatOptions.SystemPrompt));

        foreach (var msg in _discussItem.Messages)
        {
            if (!string.IsNullOrEmpty(msg.UserPrompt))
            {
                messages.Add(new(ChatRole.User, msg.UserPrompt));
            }
            if (!string.IsNullOrEmpty(msg.AiChatCompletion.Content))
            {
                messages.Add(new(ChatRole.Assistant, msg.AiChatCompletion.Content));
            }
        }
        return messages;
    }

    private static Microsoft.Extensions.AI.ChatOptions CreateChatOptions(Core.Models.ChatOptionsModel options)
    {
        var result = new Microsoft.Extensions.AI.ChatOptions()
        {
            MaxOutputTokens = options.MaxTokens,
            Temperature = options.Temperature,
            TopP = options.TopP,
            FrequencyPenalty = options.FrequencyPenalty,
            Seed = options.Seed,
            TopK = options.TopK,
        };

        result.AdditionalProperties = new()
        {
            ["thinking_budget"] = options.ThinkingBudget
        };
        return result;
    }

    private void RaiseStreamEvent(string content, UpdateType type, TokenUsageModel usage)
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
