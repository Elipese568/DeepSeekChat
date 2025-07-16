using System.Collections.Generic;
using System.Collections;

namespace DeepSeekChat.Core.AI.Internal.Request.Primitives;

public struct MultipleValue<T> : IEnumerable<T>
{
    private List<T> _values;

    public MultipleValue(params T[] values)
    {
        _values = new List<T>(values);
    }

    public MultipleValue()
    {
        _values = new List<T>();
    }

    public void Add(T value)
    {
        _values.Add(value);
    }

    public void Remove(T value)
    {
        _values.Remove(value);
    }

    public bool Contains(T value)
    {
        return _values.Contains(value);
    }

    public T[] GetValues()
    {
        return _values.ToArray();
    }

    public IEnumerator<T> GetEnumerator()
    {
        return ((IEnumerable<T>)_values).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_values).GetEnumerator();
    }
}
