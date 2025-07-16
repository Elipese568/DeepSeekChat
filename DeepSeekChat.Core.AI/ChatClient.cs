using DeepSeekChat.Core.AI.Chat.Message;
using DeepSeekChat.Core.AI.Chat.Primitives;
using DeepSeekChat.Core.AI.Chat.Tool;
using DeepSeekChat.Core.AI.Exceptions;
using DeepSeekChat.Core.AI.Internal.Request;
using DeepSeekChat.Core.AI.Internal.Response;
using DeepSeekChat.Core.AI.Internal.Response.Exceptions;
using DeepSeekChat.Core.Network.Body;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.Core.AI;

public class ChatClient
{
    private readonly HttpClient _serverHttpClient;
    public Readonly<ApiKeyCredential> ApiKey { get; }
    public Uri ServerHost { get; }
    public string Model { get; }

    internal ChatClient(AIClient aiClient, string model)
    {
        ApiKey = aiClient.ApiKey;
        ServerHost = aiClient.ServerHost;
        Model = model;

        _serverHttpClient = aiClient._serverHttpClient ?? throw new ArgumentNullException(nameof(aiClient), "AIClient's HttpClient cannot be null.");

        if (ApiKeyCredential.ApiKeyProperty.GetValue(ApiKey) is not string apiKeyValue || string.IsNullOrWhiteSpace(apiKeyValue))
        {
            throw new ArgumentException("API key cannot be null or empty.", nameof(ApiKey));
        }
    }

    private ChatCompletionRequestBody BuildRequestBody(ChatMessageCollection messages, ChatOption option, Tool[] tools, bool stream)
    {
        var messageRequestBodies = messages.Select(x => x.ToRequestBody());
        var optionRequestBody = option.ToRequestBody();
        optionRequestBody.Stream = stream;
        var toolRequestBody = tools.Select(x => x.ToRequestBody());

        return new ChatCompletionRequestBody()
        {
            AuthorizationHeader = new(ApiKeyCredential.ApiKeyProperty.GetValue(ApiKey)),
            Messages = messageRequestBodies.ToArray(),
            Model = Model,
            Options = optionRequestBody,
            Tools = toolRequestBody.ToArray()
        };
    }

    public async Task<WholeChatCompletionResponse> ChatCompletionAsync(ChatMessageCollection messages, ChatOption option, Tool[] tools)
    {
        var requestBody = BuildRequestBody(messages, option, tools, false);
        var msg = BuildRequestMessage(requestBody);

        var response = _serverHttpClient.Send(msg);
        var responseInfo = await ResponseBody.MakeResponseBodyAsync<WholeChatCompletionResponse>(response);

        if (responseInfo.IsError)
        {
            ProcessErrorThrow(responseInfo);
        }

        return (WholeChatCompletionResponse)responseInfo.ResultDto;
    }

    private static void ProcessErrorThrow(ResponseBodyInfo responseInfo)
    {
        if (responseInfo.ResultDto is GenericError400 error400)
        {
            throw new HttpRequestException(
                httpRequestError: HttpRequestError.InvalidResponse,
                message:
                    $"An error from response. \n" +
                    $"Message: {error400.Message} \n" +
                    $"Data: {error400.Data} \n" +
                    $"Code: {error400.Code}",
                statusCode: System.Net.HttpStatusCode.BadRequest);
        }
        else if (responseInfo.ResultDto is RateLimitError429 error429)
        {
            string message = $"An error from response. \n" +
                             $"Message: {error429.Message} \n" +
                             $"Data: {error429.Data}";
            if (error429.Message.Contains("maximum context length"))
                throw new TokenLimitExceededException(message);
            else
                throw new RateLimitExceededException(message);
        }
        else if (responseInfo.ResultDto is ServiceOverloadError503 error503)
        {
            throw new ServiceOverloadException(
                $"An error from response. \n" +
                $"Message: {error503.Message} \n" +
                $"Data: {error503.Data} \n" +
                $"Code: {error503.Code}");
        }
        else if (responseInfo.StatusCode is 404)
        {
            throw new HttpRequestException(HttpRequestError.InvalidResponse, message: (string)responseInfo.ResultDto, statusCode: System.Net.HttpStatusCode.NotFound);
        }
        else if (responseInfo.StatusCode is 401)
        {
            throw new HttpRequestException(HttpRequestError.UserAuthenticationError, message: (string)responseInfo.ResultDto, statusCode: System.Net.HttpStatusCode.Unauthorized);
        }
        else if (responseInfo.StatusCode is 504)
        {
            throw new HttpRequestException(HttpRequestError.ConnectionError, message: (string)responseInfo.ResultDto, statusCode: System.Net.HttpStatusCode.GatewayTimeout)
        }
    }

    private HttpRequestMessage BuildRequestMessage(ChatCompletionRequestBody requestBody)
    {
        HttpRequestMessage msg = new(HttpMethod.Post, new Uri(ServerHost, "/chat/completions"));
        var parameters = requestBody.GetBodyParameters(); // this only get two parameters: Auth., JsonContent

        foreach (var parameter in parameters)
        {
            switch (parameter.Type)
            {
                case ParameterType.RequestBody: // should only accept json content
                    {
                        var content = new StringContent((string)parameter.Value);
                        content.Headers.ContentType.MediaType = "application/json";

                        msg.Content = content;
                        break;
                    }
                case ParameterType.HttpHeader: // should only accept auth. header
                    {
                        msg.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", (string)parameter.Value);
                        break;
                    }
                default:
                    throw new NotImplementedException("What's going on?");
            }
        }

        return msg;
    }
}
