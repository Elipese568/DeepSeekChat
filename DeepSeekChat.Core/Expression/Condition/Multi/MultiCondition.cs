using System;
using System.Collections.Generic;
using System.Linq;

namespace DeepSeekChat.Core.Expression.Condition.Multi;

public class MultiCondition : Element, ICondition
{
    private List<ConditionElement> _conditions = new();
    public MultiCondition(params ConditionElement[] conditions)
    {
        if (conditions == null || conditions.Length == 0)
            throw new ArgumentException("At least one condition must be provided.", nameof(conditions));
        _conditions.AddRange(conditions);
    }
    public void AddCondition(ConditionElement condition)
    {
        if (condition == null)
            throw new ArgumentNullException(nameof(condition), "Condition cannot be null.");
        _conditions.Add(condition);
    }

    public virtual bool GetResult()
    {
        return _conditions.All(c => c.GetResult());
    }

    public sealed override object GetValue()
    {
        return GetResult();
    }

    public virtual Element[] GetElements()
    {
        return _conditions.Cast<Element>().ToArray();
    }
}

