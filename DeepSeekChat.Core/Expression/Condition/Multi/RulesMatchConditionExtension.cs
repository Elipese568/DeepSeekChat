using System;

namespace DeepSeekChat.Core.Expression.Condition.Multi;

public static class RulesMatchConditionExtension
{
    public static RulesMatchCondition ToDictionaryKey(this RulesMatchCondition condition)
    {
        ArgumentNullException.ThrowIfNull(condition);
        // Convert the condition to a dictionary key representation
        // This is a placeholder implementation; actual logic may vary
        condition.Target = new NamedElement(ConditionDictionary.KeyElementName);
        return condition;
    }
}
