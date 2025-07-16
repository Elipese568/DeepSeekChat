using DeepSeekChat.Core.Network.Body;
using DeepSeekChat.Core.Network.Body.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace DeepSeekChat.Core.AI.Internal.Request.Primitives;

public class ExtensiblePropertyRequestBody : RequestBody
{
    [BodyParameterModeIgnore(IgnoreMode.AlwaysIgnore)]
    public virtual Dictionary<string, object> ExtraParameters { get; set; } = new();

    public override IEnumerable<RequestParameter> GetBodyParameters()
    {
        var parameters = base.GetBodyParameters().ToList();
        foreach (var additionalProperty in ExtraParameters)
        {
            parameters.Add(new RequestParameter(additionalProperty.Key, additionalProperty.Value, ParameterType.RequestBody));
        }

        return parameters;
    }
}