using System;
using DeepSeekChat.Core.Network.Body;

namespace DeepSeekChat.Core.AI.Internal.Request.Primitives;

// 泛型转换器
public class MultipleValueParameterConverter<T> : IBodyParameterValueConverter
{
    public object ConvertBack(object value, Type targetvalue)
    {
        throw new NotImplementedException();
    }

    public object ConvertTo(object value)
    {
        if (value is not MultipleValue<T> multiplePropertyvalue)
        {
            throw new ArgumentException($"Value must be of value MultipleValue<{typeof(T).Name}>.", nameof(value));
        }

        try
        {
            var values = multiplePropertyvalue.GetValues();
            if (values.Length == 0)
            {
                return null;
            }

            if (values.Length == 1)
            {
                return values[0];
            }

            return values;
        }
        catch
        {
            return null;
        }
    }
}
