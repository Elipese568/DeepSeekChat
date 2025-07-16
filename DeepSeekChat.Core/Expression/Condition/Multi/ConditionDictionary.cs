using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DeepSeekChat.Core.Expression.Condition.Multi;

public static class ConditionDictionary
{
    public const string KeyElementName = "__Dic_Key";
    public static NamedElement KeyElement => new(KeyElementName);
}

public class ConditionDictionary<TValue> : IDictionary<ICondition, TValue>
{
    List<KeyValuePair<ICondition, TValue>> _conditions;

    public ConditionDictionary()
    {
        _conditions = new List<KeyValuePair<ICondition, TValue>>();
    }

    public TValue[] this[object i, int depth = 1]
    {
        get
        {
            if (i == null)
                throw new ArgumentNullException(nameof(i), "Index cannot be null.");
            if (depth < 0)
                throw new ArgumentOutOfRangeException(nameof(depth), "Depth must be non-negative.");
            return GetValuesInternal(i, depth).ToArray();
        }
    }

    public void Add(ICondition condition, TValue value)
    {
        if (condition == null)
            throw new ArgumentNullException(nameof(condition), "Condition cannot be null.");
        if (value == null)
            throw new ArgumentNullException(nameof(value), "Value cannot be null.");
        _conditions.Add(new KeyValuePair<ICondition, TValue>(condition, value));
    }

    public void Clear()
    {
        _conditions.Clear();
    }

    public bool ContainsKey(ICondition key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key), "Key cannot be null.");
        return _conditions.Any(pair => pair.Key.Equals(key));
    }

    public bool ContainsPass(object obj)
    {
        return _conditions.Any(pair =>
        {
            NamedElement.AssignNamedElement(pair.Key, ConditionDictionary.KeyElementName, obj);
            return pair.Key.GetResult();
        });
    }

    public bool Remove(ICondition key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key), "Key cannot be null.");
        return _conditions.RemoveAll(pair => pair.Key.Equals(key)) > 0;
    }

    public bool RemovePass(object obj)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj), "Object cannot be null.");

        return _conditions.RemoveAll(pair =>
        {
            NamedElement.AssignNamedElement(pair.Key, ConditionDictionary.KeyElementName, obj);
            return pair.Key.GetResult();
        }) > 0;
    }

    private IEnumerable<TValue> GetValuesInternal(ICondition condition, int depth)
    {
        if (condition == null)
            throw new ArgumentNullException(nameof(condition), "Condition cannot be null.");
        if (depth < 0)
            throw new ArgumentOutOfRangeException(nameof(depth), "Depth must be non-negative.");
        if (condition == null)
            throw new ArgumentNullException(nameof(condition), "Key cannot be null.");

        List<TValue> values = new();
        int currentDepth = 0;
        foreach (var pair in _conditions)
        {
            if (pair.Key.Equals(condition))
            {
                values.Add(pair.Value);
                if (currentDepth == depth)
                {
                    return [.. values];
                }
                currentDepth++;
            }
        }
        return [.. values];
    }

    private IEnumerable<TValue> GetValuesInternal(object i, int depth)
    {
        if (depth < 0)
            throw new ArgumentOutOfRangeException(nameof(depth), "Depth must be non-negative.");
        List<TValue> results = new();
        int currentDepth = 1;
        foreach (var pair in _conditions)
        {
            NamedElement.TryAssignNamedElement(pair.Key, ConditionDictionary.KeyElementName, i, out var _);
            if (pair.Key.GetResult())
            {
                results.Add(pair.Value);
                if (currentDepth == depth)
                {
                    return [.. results];
                }
                currentDepth++;
            }
        }

        return [.. results];
    }

    public TValue GetValue(ICondition key)
    {
        return GetValuesInternal(key, 1).First();
    }

    public IEnumerable<TValue> GetValue(ICondition key, int depth)
    {
        return GetValuesInternal(key, depth);
    }

    public bool TryGetValue(ICondition key, [MaybeNullWhen(false)] out TValue value)
    {
        var ret = GetValuesInternal(key, 1).FirstOrDefault();

        value = ret ?? default;
        return value == null;
    }

    public bool TryGetValue(ICondition key, int depth, [MaybeNullWhen(false)] out IEnumerable<TValue> value)
    {
        var ret = GetValuesInternal(key, depth);

        value = ret;
        return ret.Any();
    }

    public TValue GetValue(object obj)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj), "Object cannot be null.");
        return GetValuesInternal(obj, 1).First();
    }

    public IEnumerable<TValue> GetValue(object obj, int depth)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj), "Object cannot be null.");
        return GetValuesInternal(obj, depth);
    }

    public bool TryGetValue(object obj, int depth, [MaybeNullWhen(false)] out IEnumerable<TValue> value)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj), "Object cannot be null.");
        var ret = GetValuesInternal(obj, depth);
        value = ret;
        return value.Any();
    }

    public bool TryGetValue(object obj, [MaybeNullWhen(false)] out TValue value)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj), "Object cannot be null.");
        var ret = GetValuesInternal(obj, 1).FirstOrDefault();
        value = ret ?? default;
        return value != null;
    }

    public void SetValue(ICondition key, TValue value)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key), "Key cannot be null.");
        if (value == null)
            throw new ArgumentNullException(nameof(value), "Value cannot be null.");
        var index = _conditions.FindIndex(pair => pair.Key.Equals(key));
        if (index >= 0)
        {
            _conditions[index] = new KeyValuePair<ICondition, TValue>(key, value);
        }

        throw new KeyNotFoundException("The specified key does not exist in the dictionary.");
    }

    public void Add(KeyValuePair<ICondition, TValue> item)
    {
        _conditions.Add(item);
    }

    public bool Contains(KeyValuePair<ICondition, TValue> item)
    {
        return _conditions.Contains(item);
    }

    public void CopyTo(KeyValuePair<ICondition, TValue>[] array, int arrayIndex)
    {
        _conditions.CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<ICondition, TValue> item)
    {
        return _conditions.Remove(item);
    }

    public IEnumerator<KeyValuePair<ICondition, TValue>> GetEnumerator()
    {
        return _conditions.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerable<ICondition> Keys => _conditions.Select(pair => pair.Key);

    public IEnumerable<TValue> Values => _conditions.Select(pair => pair.Value);

    public int Count => _conditions.Count;

    ICollection<ICondition> IDictionary<ICondition, TValue>.Keys => throw new NotImplementedException();

    ICollection<TValue> IDictionary<ICondition, TValue>.Values => throw new NotImplementedException();

    public bool IsReadOnly => throw new NotImplementedException();

    public TValue this[ICondition key]
    {
        get => _conditions.Find(x => x.Key.Equals(key)).Value;
        set
        {
            if (_conditions.Any(x => x.Key.Equals(key)))
                SetValue(key, value);
            else
                Add(key, value);
        }
    }
}
