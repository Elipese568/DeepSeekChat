using DeepSeekChat.Core.Expression.Condition;
using System.Collections.Generic;
using System.Linq;

namespace DeepSeekChat.Core.Expression;

public class NamedElement : Element
{
    object _inner = default;

    public override object GetValue()
    {
        return _inner;
    }

    public void SetValue(object value)
    {
        _inner = value;
    }

    public string Name { get; }

    public NamedElement(string name)
    {
        Name = name;
    }

    public static IEnumerable<NamedElement> FindNamedElements(ICondition condition, string name)
    {
        if (condition == null)
            throw new System.ArgumentNullException(nameof(condition));
        var namedElements = new List<NamedElement>();

        foreach (var element in condition.GetElements())
        {
            if (element is NamedElement namedElement && namedElement.Name == name)
            {
                namedElements.Add(namedElement);
            }
            else if (element is ICondition subCondition)
            {
                namedElements.AddRange(FindNamedElements(subCondition, name));
            }
        }

        return namedElements;
    }

    public static IEnumerable<NamedElement> AssignNamedElement(ICondition condition, string name, object value)
    {
        System.ArgumentNullException.ThrowIfNull(condition);

        var namedElements = FindNamedElements(condition, name);
        if (namedElements.Count() == 0)
            throw new KeyNotFoundException($"No NamedElement with name '{name}' found in the condition.");

        foreach (var namedElement in namedElements)
            namedElement.SetValue(value);

        return namedElements;
    }

    public static bool TryAssignNamedElement(ICondition condition, string name, object value, out IEnumerable<NamedElement> namedElements)
    {
        if (condition == null)
        {
            namedElements = [];
            return false;
        }

        try
        {
            namedElements = AssignNamedElement(condition, name, value);
            return true;
        }
        catch
        {
            namedElements = [];
            return false;
        }
    }
}
