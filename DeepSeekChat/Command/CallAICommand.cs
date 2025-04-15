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

public record ChatCompletionMetadata(string Id, DateTime TimeCreated, ChatOptions Options);

public class CallAICommand : ICommand
{
    private const string DoneMarker = "data: [DONE]";

    private readonly string _apiKey;
    private readonly string _model;
    private readonly OpenAIClient _client;
    private readonly ChatClient _chatClient;
    private readonly DiscussItem _discussItem;
    private readonly DispatcherQueue _dispatcherQueue;

    private CancellationTokenSource _cts;
    private bool _isRunning;

    public event EventHandler CanExecuteChanged;
    public event EventHandler<ChatResponseReceivedEventArgs> StreamResponseReceived;
    public event EventHandler<ChatResponseCompletedEventArgs> StreamCompleted;
    public event EventHandler<ChatCompletionMetadata> CompletionMetadataReceived;

    public CallAICommand(string apiKey, DiscussItem discussItem, string model = "deepseek-ai/DeepSeek-R1")
    {
        _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        _model = model ?? throw new ArgumentNullException(nameof(model));
        _discussItem = discussItem ?? throw new ArgumentNullException(nameof(discussItem));

        _client = new OpenAIClient(
            new ApiKeyCredential(apiKey),
            new OpenAIClientOptions { Endpoint = new Uri("https://api.siliconflow.cn/v1/") });

        _chatClient = _client.GetChatClient(model);
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    }

    public bool CanExecute(object parameter) => !_isRunning;

    public async void Execute(object parameter)
    {
        if (parameter is not string currentPrompt || string.IsNullOrWhiteSpace(currentPrompt))
            return;

        try
        {
            IsRunning = true;
            using (_cts = new CancellationTokenSource())
            {
                var messages = BuildMessageThread(currentPrompt);
                var options = CreateChatOptions(_discussItem.ChatOptions);

                await ProcessChatStreamAsync(messages, options);
            }
        }
        catch (OperationCanceledException)
        {
            NotifyCompletion(ProgressStatus.Stoped);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Chat processing failed: {ex}");
            NotifyCompletion(ProgressStatus.Failed);
        }
        finally
        {
            IsRunning = false;
            NotifyCompletion(ProgressStatus.Completed);
        }
    }

    private List<ChatMessage> BuildMessageThread(string currentPrompt)
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
        Seed = new Func<long?>(() =>
        {
            if (options.SeedAutoRefresh)
            {
                options.Seed = Random.Shared.Next();
            }
            return options.Seed;
        })()
    };

    private async Task ProcessChatStreamAsync(List<ChatMessage> messages, ChatCompletionOptions options)
    {
        var responseStream = _chatClient.CompleteChatStreamingAsync(
            messages,
            options,
            _cts.Token);

        await foreach (var response in responseStream.GetRawPagesAsync())
        {
            var status = await ProcessChatResponseAsync(response, _cts);
            if (status != ProgressStatus.Completed)
            {
                NotifyCompletion(status);
                break;
            }
        }
    }

    private async Task<ProgressStatus> ProcessChatResponseAsync(ClientResult response, CancellationTokenSource cancellationToken)
    {
        using var streamReader = new StreamReader(
            response.GetRawResponse().ContentStream,
            Encoding.UTF8);

        try
        {
            bool isMetadataReceived = false;
            while (await streamReader.ReadLineAsync(cancellationToken.Token) is { } responseString)
            {
                cancellationToken.Token.ThrowIfCancellationRequested();
                Debug.Write(responseString);

                if (responseString == DoneMarker)
                    break;

                if (!string.IsNullOrEmpty(responseString))
                {
                    await ProcessResponseChunkAsync(responseString, isMetadataReceived);
                    isMetadataReceived = true;
                }
            }
            return ProgressStatus.Completed;
        }
        finally
        {
            streamReader.Dispose();
        }
    }

    private async Task ProcessResponseChunkAsync(string responseString, bool isMetadataReceived)
    {
        var chunk = await Task.Run(() =>
            StreamingChatCompletionChunk.FromJson(responseString.Replace("data: ","")));

        if (!isMetadataReceived)
        {
            var metadata = new ChatCompletionMetadata(chunk.Id, DateTimeOffset.FromUnixTimeSeconds(chunk.Created).LocalDateTime, _discussItem.ChatOptions);
            CompletionMetadataReceived?.Invoke(this, metadata);
            isMetadataReceived = true;
        }

        if (chunk.Choices.Count == 0)
            return;

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
