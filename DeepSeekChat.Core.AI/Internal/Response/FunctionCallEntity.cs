using System.Collections.Generic;

namespace DeepSeekChat.Core.AI.Internal.Response;

public class FunctionCallEntity
{
    public string Name { get; set; }
    public Dictionary<string, object> Arguments { get; set; }
}
