using System;

namespace DeepSeekChat.Core.Expression.Condition;

public class ConditionElement : Element, ICondition
{
    public ConditionElement(Element left, Element right)
    {
        Left = left;
        Right = right;
    }

    public sealed override object GetValue()
    {
        return GetValue();
    }

    public virtual bool GetResult() { throw new NotImplementedException(); }

    public virtual Element[] GetElements() => [Left, Right];

    public virtual Element Left { get; set; }
    public virtual Element Right { get; set; }
}
