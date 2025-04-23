using DeepSeekChat.Helper;
using DeepSeekChat.Models;
using OpenAI;
using OpenAI.Assistants;
using OpenAI.Chat;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.Service;

public class ClientUpdateEventArgs : EventArgs
{
    public string ApiKey { get; set; }
    public string Model { get; set; }
    public Uri ServerEndpoint { get; set; }
    public ChatClient ChatClient { get; set; }
}

public class ClientService
{
    private readonly SettingService _settingService;

    private OpenAIClient _client;
    private ChatClient _chatclient;
    private string _model;
    private string _apikey;
    private Uri _aiServerEndPoint;
    private List<EventHandler<ClientUpdateEventArgs>> _clientUpdateHandlers = new();

    public string ApiKey => _apikey;
    public string Model => _model;

    public event EventHandler<ClientUpdateEventArgs> ClientUpdate
    {
        add
        {
            if (!_clientUpdateHandlers.Contains(value))
                _clientUpdateHandlers.Add(value);
        }
        remove
        {
            if (_clientUpdateHandlers.Contains(value))
                _clientUpdateHandlers.Remove(value);
        }
    }

    public ClientService()
    {
        _settingService = App.Current.GetService<SettingService>();
        ConfigureAll(
            new("https://api.siliconflow.cn/v1/"), // TODO: Replace with a setting after this option is unlocked
            _settingService.Read(SettingService.SETTING_APIKEY, string.Empty),
            App.Current.GetService<ModelsManagerService>().GetModelById(new Guid(_settingService.Read(SettingService.SETTING_SELECTED_MODEL, AiModelStorage.DEEPSEEK_DEFAULT_MODEL_GUID))).ModelID);
        _settingService.SettingChanged += OnSettingChanged;
    }

    private void OnSettingChanged(object? sender, SettingChangedEventArgs e)
    {
        ConfigureAll(
            new("https://api.siliconflow.cn/v1/"), // TODO: Replace with a setting after this option is unlocked
            _settingService.Read(SettingService.SETTING_APIKEY, string.Empty),
            App.Current.GetService<ModelsManagerService>().GetModelById(new Guid(_settingService.Read(SettingService.SETTING_SELECTED_MODEL, AiModelStorage.DEEPSEEK_DEFAULT_MODEL_GUID))).ModelID);
    }

    private void NoticeUpdate()
    {
        var args = new ClientUpdateEventArgs
        {
            ApiKey = _apikey,
            Model = _model,
            ServerEndpoint = _aiServerEndPoint,
            ChatClient = _chatclient
        };
        foreach (var handler in _clientUpdateHandlers)
        {
            handler(this, args);
        }
    }

    public void UpdateApiKey(string apikey)
    {
        _apikey = apikey;
        _client = new OpenAIClient(new(apikey), new OpenAIClientOptions
        {
            Endpoint = _aiServerEndPoint
        });
        _chatclient = _client.GetChatClient(_model);
        NoticeUpdate();
    }

    public void UpdateModel(string model)
    {
        _model = model;
        _chatclient = _client.GetChatClient(_model);
        NoticeUpdate();
    }

    public void UpdateServer(Uri serverEndpoint)
    {
        _aiServerEndPoint = serverEndpoint;
        _client = new OpenAIClient(new(_apikey), new OpenAIClientOptions
        {
            Endpoint = _aiServerEndPoint
        });
        _chatclient = _client.GetChatClient(_model);
        NoticeUpdate();
    }

    public void ConfigureAll(Uri serverEndpoint, string apikey, string model)
    {
        _aiServerEndPoint = serverEndpoint;
        _apikey = apikey;
        _model = model;
        _client = new OpenAIClient(new(_apikey), new OpenAIClientOptions
        {
            Endpoint = _aiServerEndPoint
        });
        _chatclient = _client.GetChatClient(_model);
        NoticeUpdate();
    }

    public ChatClient GetChatClient()
    {
        return _chatclient;
    }
}
