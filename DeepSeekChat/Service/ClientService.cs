using DeepSeekChat.Command;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Responses;
using DeepSeekChat.Core.Models;
using DeepSeekChat.Core;

namespace DeepSeekChat.Service;

public class ClientUpdateEventArgs : EventArgs
{
    public string ApiKey { get; set; }
    public string Model { get; set; }
    public Uri ServerEndpoint { get; set; }
    public IChatClient ChatClient { get; set; }
    public bool IsClientAvailable => ChatClient != null;
}

public class DeepSeekStreamingChatCompletionUpdateAsyncEnumerable : IAsyncEnumerable<StreamingChatCompletionChunkModel>
{
    private readonly IAsyncEnumerable<ChatResponseUpdate> _clientResult;
    private const string DoneMarker = "data: [DONE]";
    private readonly CancellationTokenSource _cts;

    public DeepSeekStreamingChatCompletionUpdateAsyncEnumerable(IAsyncEnumerable<ChatResponseUpdate> clientResult, CancellationTokenSource cancellationTokenSource)
    {
        _clientResult = clientResult;
        _cts = cancellationTokenSource;
    }

    public async IAsyncEnumerator<StreamingChatCompletionChunkModel> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        if(_cts != null)
        {
            cancellationToken = _cts.Token;
        }

        await foreach(var update in _clientResult)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Debug.Write(update.Text);

            if (update.Text == DoneMarker)
                break;
            //var chunk = StreamingChatCompletionChunk.FromMaiResponse(update);
            //if(chunk.Choices.Count == 0)
            //    continue;

            //yield return chunk;
        }
        yield return null;
    }
}

public class ClientService
{
    private readonly SettingService _settingService;

    private OpenAIClient _client;
    private IChatClient _chatClient;
    private string _model;
    private string _apikey;
    private Uri _aiServerEndPoint;
    private EventHandlerWrapper<EventHandler<ClientUpdateEventArgs>> _clientUpdateHandlers;

    public string ApiKey => _apikey;
    public string Model => _model;

    public event EventHandler<ClientUpdateEventArgs> ClientUpdate
    {
        add
        {
            _clientUpdateHandlers.AddHandler(value);
        }
        remove
        {
            _clientUpdateHandlers.RemoveHandler(value);
        }
    }

    public ClientService()
    {
        _settingService = App.Current.GetService<SettingService>();
        _clientUpdateHandlers = EventHandlerWrapper<EventHandler<ClientUpdateEventArgs>>.Create();

        ApplySettingChanges();
        _settingService.SettingChanged += (_, _) => ApplySettingChanges();
    }

    private void ApplySettingChanges()
    {
        if (!string.IsNullOrWhiteSpace(_settingService.Read(SettingService.SETTING_APIKEY, string.Empty)))
            ConfigureAllAsync(
                new("https://api.siliconflow.cn/v1/"), // TODO: Replace with a setting after this option is unlocked
                _settingService.Read(SettingService.SETTING_APIKEY, string.Empty),
                App.Current.GetService<ModelsManagerService>().GetModelById(new Guid(_settingService.Read(SettingService.SETTING_SELECTED_MODEL, AiModelStorage.DEEPSEEK_DEFAULT_MODEL_GUID)))?.ModelID ?? AiModelStorage.DEEPSEEK_DEFAULT_MODEL_GUID);
    }

    public async Task<bool> IsApiKeyVaildAsync(string apiKey)
    {
        return await Task.Run(() =>
        {
            try
            {
                _ = new OpenAIClient(new(apiKey), new OpenAIClientOptions
                {
                    Endpoint = new("https://api.siliconflow.cn/v1/")
                }).GetOpenAIModelClient().GetModels();
                return true;
            }
            catch
            {
                return false;
            }
        });
    }

    private void NoticeUpdate()
    {
        var args = new ClientUpdateEventArgs
        {
            ApiKey = _apikey,
            Model = _model,
            ServerEndpoint = _aiServerEndPoint,
            ChatClient = _chatClient
        };
        _clientUpdateHandlers.Invoke(this, args);
    }

    public async void UpdateApiKeyAsync(string apikey)
    {
        if(!await IsApiKeyVaildAsync(apikey) || string.IsNullOrWhiteSpace(apikey))
        {
            _apikey = apikey;
            _client = null;
            _chatClient = null;
            NoticeUpdate();
            return;
        }

        _apikey = apikey;
        _client = new OpenAIClient(new(apikey), new OpenAIClientOptions
        {
            Endpoint = _aiServerEndPoint
        });
        _chatClient = _client.GetChatClient(_model).AsIChatClient();
        NoticeUpdate();
    }

    public void UpdateModel(string model)
    {
        _model = model;
        _chatClient = _client.GetChatClient(_model).AsIChatClient();
        NoticeUpdate();
    }

    [Obsolete("This option is locked now")]
    public void UpdateServer(Uri serverEndpoint)
    {
        _aiServerEndPoint = serverEndpoint;
        _client = new OpenAIClient(new(_apikey), new OpenAIClientOptions
        {
            Endpoint = _aiServerEndPoint
        });
        _chatClient = _client.GetChatClient(_model).AsIChatClient();
        NoticeUpdate();
    }

    public async void ConfigureAllAsync(Uri serverEndpoint, string apikey, string model)
    {
        _aiServerEndPoint = serverEndpoint;
        if (!await IsApiKeyVaildAsync(apikey) || string.IsNullOrWhiteSpace(apikey))
        {
            _apikey = apikey;
            _client = null;
            _chatClient = null;
            NoticeUpdate();
            return;
        }

        _apikey = apikey;
        _model = model;
        _client = new OpenAIClient(new(_apikey), new OpenAIClientOptions
        {
            Endpoint = _aiServerEndPoint
        });
        _chatClient = _client.GetChatClient(_model).AsIChatClient();
        NoticeUpdate();
    }

    public IChatClient GetChatClient()
    {
        return _chatClient;
    }

    public async Task<DeepSeekStreamingChatCompletionUpdateAsyncEnumerable> CompleteChatStreamingAsync(List<ChatMessage> messages, Microsoft.Extensions.AI.ChatOptions options, CancellationTokenSource cancellationTokenSource)
    {
        if(bool.Parse(_settingService.Read(SettingService.SETTING_IS_USE_DISPLAY_LANGUAGE_ANSWER, "true")))
            messages.Insert(0, new ChatMessage(ChatRole.System, $"!!IMPORTANTS: Use language '{_settingService.Read(SettingService.SETTING_DISPLAY_LANGUAGE, "zh-Hans-CN")}' to answer and reply user's prompts!!"));
        var responseStream = _chatClient.GetStreamingResponseAsync(
            messages,
            options,
            cancellationTokenSource.Token);

        return new DeepSeekStreamingChatCompletionUpdateAsyncEnumerable(responseStream, cancellationTokenSource);
    }
}
