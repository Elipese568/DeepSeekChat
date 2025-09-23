using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using DeepSeekChat.Models;
using DeepSeekChat.Service;
using DeepSeekChat.Views;
using Microsoft.UI.Dispatching;
using OpenAI.Chat;
using Windows.Devices.Sms;

namespace DeepSeekChat.Command;

[Flags]
public enum UpdateType
{
    Reasoning,
    Content,
    FunctionCalling,
    ReturnValue = 0b100
}

public record ChatResponseReceivedEventArgs(string ContentUpdate, UpdateType Type, TokenUsage TokenUsage);
public record ChatResponseCompletedEventArgs(ProgressStatus Status);

public record ChatCompletionMetadata(string Id, DateTime TimeCreated, string Model, ChatOptions Options, List<string> Mods);

public record ChatResponseFunctionCallingReceivedEventArgs(object Data, UpdateType Type, TokenUsage TokenUsage);

public class NoClientException : Exception
{
    public NoClientException(string message) : base(message)
    {
    }
}

public class ToolParameter
{
    public string Type { get; init; }
    public string Description { get; set; }
    public Dictionary<string, ToolParameter> Properties { get; set; } = new();

    protected virtual void WriteJson(Utf8JsonWriter jsonWriter)
    {
        jsonWriter.WriteString("type", Type);
        if(!string.IsNullOrEmpty(Description))
            jsonWriter.WriteString("description", Description);
        if (Properties.Count > 0)
        {
            jsonWriter.WriteStartObject("properties");
            foreach (var property in Properties)
            {
                jsonWriter.WritePropertyName(property.Key);
                jsonWriter.WriteRawValue(property.Value.ToString());
            }
            jsonWriter.WriteEndObject();
        }
    }

    public override string ToString()
    {
        MemoryStream memoryStream = new();
        Utf8JsonWriter jsonWriter = new(memoryStream);
        jsonWriter.WriteStartObject();
        WriteJson(jsonWriter);
        jsonWriter.WriteEndObject();
        jsonWriter.Flush();
        return Encoding.UTF8.GetString(memoryStream.ToArray());
    }

    public BinaryData ToBinaryData()
    {
        return new BinaryData(ToString());
    }
}

public class ObjectStructuredToolParameter : ToolParameter
{
    private List<string> _required;

    public IList<string> Required
    {
        get
        {
            return (_required ??= []);
        }
        set
        {
            _required = [.. value];
        }
    }

    public bool AdditionalProperties { get; set; } = false;

    protected override void WriteJson(Utf8JsonWriter jsonWriter)
    {
        base.WriteJson(jsonWriter);
        if (_required != null && _required.Count > 0)
        {
            jsonWriter.WriteStartArray("required");
            foreach (var item in _required)
            {
                jsonWriter.WriteStringValue(item);
            }
            jsonWriter.WriteEndArray();
        }
        jsonWriter.WriteBoolean("additionalProperties", AdditionalProperties);
    }

    public static ObjectStructuredToolParameter Create(Dictionary<string, ToolParameter> members, string[] required, string description = "", bool additionalProperties = false)
    {
        return new ObjectStructuredToolParameter
        {
            Type = "object",
            Description = description,
            Properties = members ?? new Dictionary<string, ToolParameter>(),
            AdditionalProperties = additionalProperties,
            Required = required
        };
    }
}

public enum ParameterType
{
    String,
    Number,
    Integer,
    Boolean,
    Array,
    Object
}

public static class ParameterTypeExtension
{
    public static string GetString(this ParameterType type) => Enum.GetName(typeof(ParameterType), type)?.ToLowerInvariant() ?? "string";
}

public class ArrayToolParameter : ToolParameter
{
    public ParameterType ItemType { get; set; }

    public static ArrayToolParameter Create(ParameterType itemType, string description = "")
    {
        return new ArrayToolParameter
        {
            Type = "array",
            ItemType = itemType,
            Description = description
        };
    }

    protected override void WriteJson(Utf8JsonWriter jsonWriter)
    {
        base.WriteJson(jsonWriter);
        jsonWriter.WriteStartObject("items");
        jsonWriter.WriteString("type", ItemType.GetString());
        jsonWriter.WriteEndObject();
    }
}

public class ValueToolParameter : ToolParameter
{
    public static ValueToolParameter Create(ParameterType type, string description = "")
    {
        return new ValueToolParameter
        {
            Type = type.GetString(),
            Description = description
        };
    }
}

public class EnumToolParameter : ValueToolParameter
{
    private List<string> _enum;

    public IList<string> EnumValues
    {
        get
        {
            return (_enum ??= []);
        }
        set
        {
            _enum = [.. value];
        }
    }

    public static EnumToolParameter Create(string description, params string[] enumValues)
    {
        var parameter = new EnumToolParameter
        {
            Type = "string",
            Description = description
        };

        parameter.EnumValues = enumValues;

        return parameter;
    }

    public static EnumToolParameter Create<TEnum>(string description)
        where TEnum : Enum
    {
        var enumValues = Enum.GetNames(typeof(TEnum));
        return Create(description, enumValues);
    }

    protected override void WriteJson(Utf8JsonWriter jsonWriter)
    {
        base.WriteJson(jsonWriter);
        jsonWriter.WriteStartArray("enum");
        foreach (var value in EnumValues)
        {
            jsonWriter.WriteStringValue(value);
        }
        jsonWriter.WriteEndArray();
    }
}

public class FunctionToolParameter : ObjectStructuredToolParameter
{
}

public class ExecuteAICommand : ICommand
{
    private const string DoneMarker = "data: [DONE]";

    private readonly DiscussionItem _discussItem;
    private readonly DispatcherQueue _dispatcherQueue;
    private readonly ClientService _clientService;

    private CancellationTokenSource _cts;
    private bool _isRunning;

    public event EventHandler? CanExecuteChanged;
    public event EventHandler<ChatResponseReceivedEventArgs> StreamResponseReceived;
    public event EventHandler<ChatResponseCompletedEventArgs> StreamCompleted;
    public event EventHandler<ChatCompletionMetadata> CompletionMetadataReceived;
    public event EventHandler<ChatResponseFunctionCallingReceivedEventArgs> FunctionCallingResponseReceived;

    public ExecuteAICommand(DiscussionItem discussItem)
    {
        _discussItem = discussItem ?? throw new ArgumentNullException(nameof(discussItem));
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        var settingService = App.Current.GetService<SettingService>();

        
        _clientService = App.Current.GetService<ClientService>()!;
    }

    public bool CanExecute(object? parameter) => !_isRunning && _clientService.GetChatClient() != null;

    public async void Execute(object? parameter)
    {
        try
        {
            IsRunning = true;
            CompletionMetadataReceived?.Invoke(this, new("No Received", DateTime.Now, _clientService.Model, _discussItem.ChatOptions, []));

            using (_cts = new CancellationTokenSource())
            {
            BuildSendMessage:
                var messages = BuildMessageThread(_discussItem);
                var options = CreateChatOptions(_discussItem);

                if (_discussItem.ChatOptions.StreamingOutput)
                {
                    await StreamingCompletionProcess(messages, options);
                }
                else
                {
                    var retry = await CompletionProcess(messages, options);
                    if (retry)
                        goto BuildSendMessage;
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

    private List<string> GetMods(ChatOptions options)
    {
        List<string> mods = [];
        if(options.DetailedRequest)
            mods.Add(ModIdToDescriptiveConverter.DetailedRequest);
        return mods;
    }

    private async Task<bool> CompletionProcess(List<ChatMessage> messages, ChatCompletionOptions options)
    {
        var response = await _clientService.CompleteChatAsync(messages, options, _cts);
        CompletionMetadataReceived?.Invoke(this, new(response.Id, DateTimeOffset.FromUnixTimeSeconds(response.Created).LocalDateTime, _clientService.Model, _discussItem.ChatOptions, GetMods(_discussItem.ChatOptions)));

        var choice = response.Choices[0];

        if(!string.IsNullOrEmpty(choice.Message.ReasoningContent))
        {
            RaiseStreamEvent(choice.Message.ReasoningContent, UpdateType.Reasoning, response.Usage);
        }
        
        if(!string.IsNullOrEmpty(choice.Message.Content))
        {
            RaiseStreamEvent(choice.Message.Content, UpdateType.Content, response.Usage);
        }

        if(choice.FinishReason == "tool_call")
        {
            var tools = choice.Message.ToolCallingItems;
            foreach(ToolCallingItem tool in tools)
            {
                RaiseFunctionCallingEvent(tool, UpdateType.Content | UpdateType.FunctionCalling, response.Usage);
                switch(tool.Function.Name)
                {
                    case "GetFile":
                        var fileName = ((JsonElement)tool.Function.Arguments["fileName"]).GetString();
                        RaiseFunctionCallingEvent(_discussItem.Files.Find(x => x.Name == fileName).Content, UpdateType.ReturnValue, response.Usage);
                        break;
                }
            }
            return true;
        }
        
        return false;
    }

    private async Task StreamingCompletionProcess(List<ChatMessage> messages, ChatCompletionOptions options)
    {
        bool isMetadataReported = false;
        await foreach (var chunk in await _clientService.CompleteChatStreamingAsync(messages, options, _cts))
        {
            if (!isMetadataReported)
            {
                CompletionMetadataReceived?.Invoke(this, new(chunk.Id, DateTimeOffset.FromUnixTimeSeconds(chunk.Created).LocalDateTime, _clientService.Model, _discussItem.ChatOptions, GetMods(_discussItem.ChatOptions)));
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
    }

    private List<ChatMessage> BuildMessageThread(DiscussionItem item)
    {
        return [
            .. BuildFilesMessage(item),
            .. BuildUserAssistantMessage(item)
        ];
    }

    private static List<ChatMessage> BuildUserAssistantMessage(DiscussionItem item)
    {
        List<ChatMessage> messages = new();
        messages.Add(SystemChatMessage.CreateSystemMessage(item.ChatOptions.SystemPrompt));

        foreach (var msg in item.Messages)
        {
            if (!string.IsNullOrWhiteSpace(msg.CurrentMessageMetadata.Id))
                messages.Add($"[id] {(msg.Id == item.Messages.Last().Id? "Current" : msg.CurrentMessageMetadata.Id)}");
            if(item.ChatOptions.DetailedRequest)
            {
                messages.Add(SystemChatMessage.CreateSystemMessage($"""
                    This is a detailed request message, please pay attention to the following information:
                    [creation_time] {msg.CurrentMessageMetadata.TimeCreated}
                    [completion_status] {msg.ProgressStatus} (Stoped meaning user stopped generate, Failed meaning some error occurred during generate, LengthTerminated meaning AI was stopped because generate content to long)
                    [model] {msg.CurrentMessageMetadata.Model}
                    [system_prompt] "{msg.CurrentMessageMetadata.Options.SystemPrompt}"
                    [temperature] {msg.CurrentMessageMetadata.Options.Temperature}
                    [top_p] {msg.CurrentMessageMetadata.Options.TopP}
                    [frequency_penalty] {msg.CurrentMessageMetadata.Options.FrequencyPenalty}
                    [seed] {msg.CurrentMessageMetadata.Options.Seed}
                    [mods] {string.Join(", ", msg.CurrentMessageMetadata.Mods ?? [])}
                    """));
            }
            if(msg.ReferMessage != null)
            {
                messages.Add(SystemChatMessage.CreateSystemMessage($"This message refered message which id is: {msg.ReferMessage.Id}"));
            }
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

    private static List<ChatMessage> BuildFilesMessage(DiscussionItem item)
    {
        if (item.Files?.Count == 0)
            return [];

        List<ChatMessage> messages = new();

        // provide a templete to help ai understand the files
        List<ChatMessageContentPart> parts = new List<ChatMessageContentPart>()
        {
            ChatMessageContentPart.CreateTextPart("""
            The following files are provided for context in this template (contents in {} is variable):
            [file name]: {file_name}
            [file content begin]
            {file_content}
            [file content end]

            Now the user is asking a question, please answer the question based on the context provided by the files which user selected (if there's not content, maybe user do not selected any file and you need to get):
            """)
        };

        // add each file content to the system prompt
        foreach (var file in item.Files)
        {
            parts.Add(ChatMessageContentPart.CreateTextPart($"""
                [file name]: {file.Name}
                [file content begin]
                {file.Content}
                [file content end]
                """));
        }

        //if(!item.ChatOptions.StreamingOutput)
        //{
        //    // add a note about how to handle files not provided in the system prompt
        //    //messages.Add(SystemChatMessage.CreateSystemMessage("By the way, if some files not provided in these system prompt but user used them, you can call \"GetFile\" tool to get files content."));
        //    messages.Add(SystemChatMessage.CreateSystemMessage($"You can get these following files: {string.Join(',', item.Files.Select(x => x.Name))}"));
        //}

        messages.Add(SystemChatMessage.CreateSystemMessage(parts));
        return messages;
    }

    private static ChatCompletionOptions CreateChatOptions(DiscussionItem item)
    {
        var options = item.ChatOptions;

        var result = new ChatCompletionOptions()
        {
            MaxOutputTokenCount = options.MaxTokens,
            Temperature = options.Temperature,
            TopP = options.TopP,
            FrequencyPenalty = options.FrequencyPenalty,
            Seed = options.Seed
        };

        if(item.Files?.Count > 0)
        {
            var getFileTool = ChatTool.CreateFunctionTool("GetFile",
                "Get the content of a file by its name.",
                FunctionToolParameter.Create(
                    new Dictionary<string, ToolParameter>()
                    {
                        ["fileName"] = ValueToolParameter.Create(ParameterType.String, "The name of the file to get content.")
                    },
                    ["fileName"]
                ).ToBinaryData(),
                true
            );

            result.Tools.Add(getFileTool);
        }

        return result;
    }

    private void RaiseStreamEvent(string content, UpdateType type, TokenUsage usage)
    {
        StreamResponseReceived?.Invoke(this, new(content, type, usage));
    }

    private void RaiseFunctionCallingEvent(object data, UpdateType type, TokenUsage usage)
    {
        FunctionCallingResponseReceived?.Invoke(this, new(data, type, usage));
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
