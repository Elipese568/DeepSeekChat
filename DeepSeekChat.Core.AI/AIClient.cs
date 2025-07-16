using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.Core.AI;

public class AIClientCreateOption
{
    public Uri ServerHost { get; set; }
    public ApiKeyCredential ApiKey { get; set; }
}

public class AIClient
{
    internal readonly HttpClient _serverHttpClient;

    public Uri ServerHost { get; }
    public Readonly<ApiKeyCredential> ApiKey { get; }

    private AIClient(Uri host, ApiKeyCredential apiKey)
    {
        ArgumentNullException.ThrowIfNull(host, nameof(host));
        ArgumentNullException.ThrowIfNull(apiKey, nameof(apiKey));

        ServerHost = host;
        ApiKey = apiKey.AsReadonly();

        _serverHttpClient = new();
    }

    public async Task<bool> ClientIsAvaliable(Uri? testUri = null, HttpMethod? method = null)
    {
        if(ApiKeyCredential.ApiKeyProperty.GetValue(ApiKey) is not string apikeyValue || string.IsNullOrWhiteSpace(apikeyValue))
        {
            return false;
        }

        if(testUri != null)
        {
            try
            {
                var request = new HttpRequestMessage(method??HttpMethod.Get, testUri.IsAbsoluteUri? testUri : new(ServerHost, testUri));
                request.Headers.Add("Authorization", $"Bearer {apikeyValue}");

                using var response = await _serverHttpClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        return true;
    }

    public ChatClient GetChatClient(string model)
    {
        return new ChatClient(this, model);
    }

    public static AIClient Create(AIClientCreateOption options) => new(options.ServerHost, options.ApiKey);
}
