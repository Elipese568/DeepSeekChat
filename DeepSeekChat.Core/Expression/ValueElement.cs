using System;

namespace DeepSeekChat.Core.Expression;

public class ValueElement<TValue> : Element
{
    private TValue _inner;

    public override object GetValue()
    {
        return _inner;
    }

    public ValueElement(TValue value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value), "Value cannot be null.");
        _inner = value;
    }
}

public static class ValueElement
{
    public static ValueElement<T> MakeValueElement<T>(T value) => new(value);
}
