using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using DeepSeekChat.Models;
using Microsoft.UI.Dispatching;
using OpenAI;
using OpenAI.Chat;

namespace DeepSeekChat.Command;

public class CallAICommand : ICommand
{
    private readonly string _apiKey;
    private readonly string _model;
    private readonly OpenAIClient _client;
    private readonly ChatClient _chatClient;

    private readonly DiscussItem _discussItem;
    private CancellationTokenSource _cts;
    private bool _isRunning;
    private readonly DispatcherQueue _dispatcherQueue;

    public event EventHandler CanExecuteChanged;
    public event EventHandler<string> StreamResponseReceived;
    public event EventHandler StreamCompleted;

    public CallAICommand(string apiKey, DiscussItem discussItem, string model = "deepseek-ai/DeepSeek-R1")
    {
        _apiKey = apiKey;
        _model = model;
        _discussItem = discussItem;
        _client = new OpenAIClient(new System.ClientModel.ApiKeyCredential(apiKey), new OpenAIClientOptions()
        {
            Endpoint = new Uri("https://api.siliconflow.cn/v1/")
        });
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
            _cts = new CancellationTokenSource();

            // 构建历史消息
            var messages = new List<ChatMessage>();
            messages.Add(SystemChatMessage.CreateSystemMessage("现在你是一个名为\"Deepseek R1\"的通用推理模型，请使用Markdown格式回答用户的Prompt，并且在<think></think>标签里输出你的思考过程"));
            foreach (var msg in _discussItem.Messages)
            {
                if (!string.IsNullOrEmpty(msg.UserPrompt))
                {
                    messages.Add(UserChatMessage.CreateUserMessage(msg.UserPrompt));
                }
                if (!string.IsNullOrEmpty(msg.AiAnswer))
                {
                    messages.Add(AssistantChatMessage.CreateAssistantMessage(msg.AiAnswer));
                }
            }

            // 添加当前新消息
            messages.Add(UserChatMessage.CreateUserMessage(currentPrompt));

            var responseStream = _chatClient.CompleteChatStreamingAsync(messages, options: new ChatCompletionOptions()
            {
                MaxOutputTokenCount = 8192,
                Temperature = 0.6F,
                TopP = 0.95F,
                FrequencyPenalty = 0.0F,
                Seed = Random.Shared.Next(0, int.MaxValue)
            }, cancellationToken: _cts.Token);

            var fullResponse = new StringBuilder();
            await foreach (StreamingChatCompletionUpdate response in responseStream)
            {
                var contentText = string.Join("", response.ContentUpdate.Select(x => x.Text));
                if (!string.IsNullOrEmpty(contentText))
                {
                    fullResponse.Append(contentText);
                    _dispatcherQueue.TryEnqueue(() =>
                    {
                        StreamResponseReceived?.Invoke(this, fullResponse.ToString());
                    });
                }
            }
        }
        catch (OperationCanceledException)
        {
            // 处理取消
        }
        finally
        {
            IsRunning = false;
            _cts?.Dispose();
            _dispatcherQueue.TryEnqueue(() => StreamCompleted?.Invoke(this, EventArgs.Empty));
        }
    }

    public bool IsRunning
    {
        get => _isRunning;
        set
        {
            if (_isRunning != value)
            {
                _isRunning = value;
                _dispatcherQueue.TryEnqueue(() =>
                {
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                });
            }
        }
    }
}
