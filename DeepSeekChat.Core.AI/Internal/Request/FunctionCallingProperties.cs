using DeepSeekChat.Core.AI.Internal.Request.Primitives;
using DeepSeekChat.Core.Expression;
using DeepSeekChat.Core.Expression.Condition;
using DeepSeekChat.Core.Network.Body;
using DeepSeekChat.Core.Network.Body.Attributes;
using System.Collections.Generic;

namespace DeepSeekChat.Core.AI.Internal.Request;

public class FunctionCallingProperties : ExtensiblePropertyRequestBody
{
    public static NotEqualCondition ObjectRequiredIgnoreCondition = new(new NamedElement("Type"), new ValueElement<string>("object"));

    public FunctionCallingProperties() { }

    [BodyParameterModeIgnore(IgnoreMode.WhenStringEmptyIgnore)]
    public string Name { get; set; }

    [BodyItemConverter(typeof(MultipleValueParameterConverter<string>))]
    public MultipleValue<string> Type { get; set; }

    [BodyParameterModeIgnore(IgnoreMode.WhenStringEmptyIgnore)]
    public string Description { get; set; }

    [BodyItemCollectionElementType(typeof(string))]
    [BodyParameterModeIgnore(IgnoreMode.WhenArrayEmptyIgnore)]
    public string[] Required { get; set; }

    [BodyItemDictionaryValueType(typeof(FunctionCallingProperties))]
    [BodyParameterModeIgnore(IgnoreMode.WhenCollectionEmptyIgnore)]
    public Dictionary<string, FunctionCallingProperties> Properties { get; set; } = [];

    public bool AdditionalProperties { get; set; }
}
