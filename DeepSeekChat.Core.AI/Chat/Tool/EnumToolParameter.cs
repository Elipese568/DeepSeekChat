using DeepSeekChat.Core.AI.Internal.Request;
using System.Collections.Generic;

namespace DeepSeekChat.Core.AI.Chat.Tool;

public class EnumToolParameter<T> : ValueToolParameter<T>
{
    public List<T> Values { get; set; }

    public EnumToolParameter() : base()
    {
        Values = new List<T>();
    }

    public override FunctionCallingProperties ToRequestBody()
    {
        var req = base.ToRequestBody();

        req.ExtraParameters.Add("enum", Values);
        return req;
    }
}
