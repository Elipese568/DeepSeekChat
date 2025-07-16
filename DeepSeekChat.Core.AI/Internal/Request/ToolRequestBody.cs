using DeepSeekChat.Core.AI.Internal.Request.Primitives;
using DeepSeekChat.Core.Expression;
using DeepSeekChat.Core.Expression.Condition;
using DeepSeekChat.Core.Network.Body;
using DeepSeekChat.Core.Network.Body.Attributes;

namespace DeepSeekChat.Core.AI.Internal.Request;

public class ToolRequestBody : ExtensiblePropertyRequestBody
{
    public static NotEqualCondition FunctionCallingParametersIgnoreCondition = new(new NamedElement("Type"), new ValueElement<string>("function"));

    public string Type { get; set; }

    [BodyParameterConditionIgnore(nameof(FunctionCallingParametersIgnoreCondition))]
    public string Name { get; set; }

    [BodyParameterConditionIgnore(nameof(FunctionCallingParametersIgnoreCondition))]
    public string Description { get; set; }

    [BodyParameterConditionIgnore(nameof(FunctionCallingParametersIgnoreCondition))]
    public FunctionCallingProperties Parameters { get; set; }

    [BodyParameterConditionIgnore(nameof(FunctionCallingParametersIgnoreCondition))]
    public bool Strict { get; set; }
}
