using DeepSeekChat.Core.Helpers;
using System;

namespace DeepSeekChat.Core.Expression.Condition;

public class TypeMatchCondition : ConditionElement
{
    public bool ConditionMatchBasedOnType { get; set; }
    public TypeMatchCondition(Element left, Element right, bool _conditionMatchBasedOnType = false) : base(left, right)
    {
        ConditionMatchBasedOnType = _conditionMatchBasedOnType;
    }

    public override bool GetResult()
    {
        if (Left == null || Right == null)
            return false;
        Type leftType = Left.GetValue() as Type;
        Type rightType = Right.GetValue() as Type;
        if (leftType == null || rightType == null)
            return false;

        if (ConditionMatchBasedOnType)
            return leftType.IsBasedOn(rightType) || rightType.IsBasedOn(leftType);

        return leftType == rightType;
    }
}


