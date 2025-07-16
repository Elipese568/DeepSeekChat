namespace DeepSeekChat.Core.AI.Internal.Response;

public class ToolCall
{
    public string Id { get; set; }
    public string Type { get; set; }
    public FunctionCallEntity FunctionCall { get; set; }
}
