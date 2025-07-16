using DeepSeekChat.Core.Network.Body;
using DeepSeekChat.Core.Network.Body.Attributes;

namespace DeepSeekChat.Core.AI.Internal.Request.Primitives;

public class AuthorizationOperationBodyBase : JsonContentTypeRequestBodyBase
{
    [BodyItemConverter(typeof(AuthorizationHeaderConverter))]
    [BodyParameterType(ParameterType.HttpHeader)]
    [BodyParameterName("Authorization")]
    public AuthorizationHeader AuthorizationHeader { get; set; }
}
