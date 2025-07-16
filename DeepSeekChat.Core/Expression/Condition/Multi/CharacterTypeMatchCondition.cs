using System;

namespace DeepSeekChat.Core.Expression.Condition.Multi;

public class CharacterTypeMatchCondition : AnyRulesMatchCondition
{
    public static CharacterTypeMatchCondition Equal { get; } = new(TypeRule.Equal);
    public static CharacterTypeMatchCondition NotEqual { get; } = new(TypeRule.NotEqual);
    private CharacterTypeMatchCondition(TypeRule rule)
        : base([
            rule.ValueConstructingSet(ValueElement.MakeValueElement(typeof(char))),
            rule.ValueConstructingSet(ValueElement.MakeValueElement(typeof(string))),
            rule.ValueConstructingSet(ValueElement.MakeValueElement(typeof(ReadOnlyMemory<char>))),
            rule.ValueConstructingSet(ValueElement.MakeValueElement(typeof(ReadOnlySpan<char>))),
            rule.ValueConstructingSet(ValueElement.MakeValueElement(typeof(char[])))
        ])
    { }
}
