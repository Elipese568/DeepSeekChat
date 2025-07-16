using DeepSeekChat.Core.AI.Internal.Request;
using System.Collections.Generic;

namespace DeepSeekChat.Core.AI.Chat.Tool;

public class StructuredObjectToolParameter : ToolParameter
{
    public List<ToolParameter> Members { get; set; }

    public List<string> Required { get; set; }
    public StructuredObjectToolParameter()
    {
        Members = new List<ToolParameter>();
        Required = new List<string>();
    }

    public override string[] Type => ["object"];

    public override FunctionCallingProperties ToRequestBody()
    {
        var properites = new Dictionary<string, FunctionCallingProperties>();

        foreach (var member in Members)
        {
            var body = member.ToRequestBody();
            var name = body.Name;
            body.Name = "";

            properites.Add(name, body);
        }

        return new()
        {
            Name = Name,
            Description = Description,
            AdditionalProperties = false,
            Properties = properites,
            Required = Required.ToArray(),
            Type = ["object"],
        };
    }
}
