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

public class ClientService
{
    private readonly OpenAIClient _client;
    private readonly Assistant _assistant;

    public string ApiKey { get; init; }
    public string ModelId { get; init; }
    public Uri AiServerEndPoint { get; init; }

    public ClientService(string apiKey, string model, Uri aiServerEndPoint)
    {
        ApiKey = apiKey;
        ModelId = model;

        _client = new(new ApiKeyCredential(apiKey), new()
        {
            Endpoint = aiServerEndPoint,
        });

        AiServerEndPoint = aiServerEndPoint;
        
        var assistantClient = _client.GetAssistantClient();
    }


}
