using System;

namespace DeepSeekChat.Core.Network.Body;

public interface IBodyParameterValueConverter
{
    public object ConvertBack(object value, Type targetType);
    public object ConvertTo(object value);
}
