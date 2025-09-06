using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenAI;
using OpenAI.Chat;
using DeepSeekChat.Helper.Converters;
using DeepSeekChat.Helper;
using Microsoft.UI.Xaml.Media;
using System.Text.Json.Serialization;
using DeepSeekChat.Command;
using System.Text.Json;

namespace DeepSeekChat.Models;

public enum ProgressStatus
{
    InProgress,
    Completed,
    Stoped,
    LengthTerminated,
    Failed,
    None = -1
}

public class ContentPartConverter : JsonConverter<ContentPart>
{
    public override ContentPart? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected start of object.");
        }
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            var root = doc.RootElement;
            if (root.TryGetProperty("Type", out var typeProp))
            {
                string type = typeProp.GetString();
                return type switch
                {
                    "text" => JsonSerializer.Deserialize<TextContentPart>(root.GetRawText(), options),
                    "tool_calling" => JsonSerializer.Deserialize<ToolCallingContentPart>(root.GetRawText(), options),
                    _ => throw new JsonException($"Unknown content part type: {type}")
                };
            }
            else
            {
                throw new JsonException("Missing 'type' property in content part.");
            }
        }
    }
    public override void Write(Utf8JsonWriter writer, ContentPart value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}

[JsonConverter(typeof(ContentPartConverter))]
public class ContentPart
{
    // required: text, tool_calling
    public string Type { get; init; }
}


public class TextContentPart : ContentPart
{
    public string Text { get; set; }
    public TextContentPart()
    {
        Type = "text";
    }
}

public class ToolCallingContentPart : ContentPart
{
    public string Name { get; set; }
    public Dictionary<string, string> Arguments { get; set; }

    public string Result { get; set; }
    public string Id { get; set; }
}

public partial class AiChatCompletion : ObservableObject
{
    public string ReasoningContent { get; set; }

    public string Content { get; set; }
}

public partial class ApplicationChatMessage : ObservableObject
{
    public string UserPrompt { get; set; }

    public AiChatCompletion AiChatCompletion { get; set; }

    public TokenUsage TokenUsage { get; set; }

    public ChatCompletionMetadata CurrentMessageMetadata { get; set; }
    public ApplicationChatMessage ReferMessage { get; set; }

    public string Id { get; set; }

    public ProgressStatus ProgressStatus { get; set; }

    public ApplicationChatMessage()
    {
        Id = Guid.NewGuid().ToString();
    }
}
