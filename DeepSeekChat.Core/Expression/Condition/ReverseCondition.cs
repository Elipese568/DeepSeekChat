using System;

namespace DeepSeekChat.Core.Expression.Condition;

public class ReverseCondition : ICondition
{
    public ConditionElement _condition;

    public ReverseCondition(ConditionElement condition)
    {
        _condition = condition ?? throw new ArgumentNullException(nameof(condition));
    }

    public Element[] GetElements()
    {
        return ((ICondition)_condition).GetElements();
    }

    public bool GetResult()
    {
        return !_condition.GetResult();
    }
}
