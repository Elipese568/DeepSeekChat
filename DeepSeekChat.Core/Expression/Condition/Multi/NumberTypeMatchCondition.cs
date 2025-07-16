using System;

namespace DeepSeekChat.Core.Expression.Condition.Multi;

public class NumberTypeMatchCondition : AnyRulesMatchCondition
{
    public static NumberTypeMatchCondition Equal { get; } = new(TypeRule.Equal);
    public static NumberTypeMatchCondition NotEqual { get; } = new(TypeRule.NotEqual);
    private NumberTypeMatchCondition(TypeRule rule)
        : base([
            rule.ValueConstructingSet(ValueElement.MakeValueElement(typeof(int))),
            rule.ValueConstructingSet(ValueElement.MakeValueElement(typeof(long))),
            rule.ValueConstructingSet(ValueElement.MakeValueElement(typeof(float))),
            rule.ValueConstructingSet(ValueElement.MakeValueElement(typeof(double))),
            rule.ValueConstructingSet(ValueElement.MakeValueElement(typeof(decimal))),
            rule.ValueConstructingSet(ValueElement.MakeValueElement(typeof(sbyte))),
            rule.ValueConstructingSet(ValueElement.MakeValueElement(typeof(byte))),
            rule.ValueConstructingSet(ValueElement.MakeValueElement(typeof(short))),
            rule.ValueConstructingSet(ValueElement.MakeValueElement(typeof(ushort))),
            rule.ValueConstructingSet(ValueElement.MakeValueElement(typeof(uint))),
            rule.ValueConstructingSet(ValueElement.MakeValueElement(typeof(ulong))),
            rule.ValueConstructingSet(ValueElement.MakeValueElement(typeof(nint))),
            rule.ValueConstructingSet(ValueElement.MakeValueElement(typeof(nuint))),
            rule.ValueConstructingSet(ValueElement.MakeValueElement(typeof(Int128))),
            rule.ValueConstructingSet(ValueElement.MakeValueElement(typeof(UInt128)))
        ])
    { }


}