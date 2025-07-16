using DeepSeekChat.Core.AI.Internal.Request;
using DeepSeekChat.Core.Network.Request;

namespace DeepSeekChat.Core.AI.Chat.Tool;

public abstract class ToolParameter : IRequestBodyConvertable<FunctionCallingProperties>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public abstract string[] Type { get; }

    public virtual FunctionCallingProperties ToRequestBody()
    {
        return new()
        {
            Name = Name,
            Description = Description,
            Type = [..Type],
            AdditionalProperties = false
        };
    }
}
