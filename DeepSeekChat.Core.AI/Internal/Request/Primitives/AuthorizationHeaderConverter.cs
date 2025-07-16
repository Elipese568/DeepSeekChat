using DeepSeekChat.Core.Network.Body;
using System;

namespace DeepSeekChat.Core.AI.Internal.Request.Primitives;

public sealed class AuthorizationHeaderConverter : IBodyParameterValueConverter
{
    public object ConvertBack(object value, Type targetType)
    {
        throw new NotImplementedException();
    }

    public object ConvertTo(object value)
    {
        return "Bearer " + ((AuthorizationHeader)value).ApiKey;
    }
}
