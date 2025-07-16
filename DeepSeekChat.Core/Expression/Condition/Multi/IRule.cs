using System;

namespace DeepSeekChat.Core.Expression.Condition.Multi;

public interface IRule
{
    public ICondition ConstructCondition(Element target);
    public Element ConditionRuleElement { get; set; }
}

public struct EqualRule : IRule
{
    private Element _conditionRuleElement;
    public Element ConditionRuleElement
    {
        get => _conditionRuleElement;
        set
        {
            _conditionRuleElement = value;
        }
    }
    public ICondition ConstructCondition(Element target)
    {
        ArgumentNullException.ThrowIfNull(target);
        return new EqualCondition(target, ConditionRuleElement);
    }
}

public struct NotEqualRule : IRule
{
    private Element _conditionRuleElement;
    public Element ConditionRuleElement
    {
        get => _conditionRuleElement;
        set
        {
            _conditionRuleElement = value;
        }
    }
    public ICondition ConstructCondition(Element target)
    {
        ArgumentNullException.ThrowIfNull(target);
        return new NotEqualCondition(target, ConditionRuleElement);
    }
}

public struct TypeRule : IRule
{
    private readonly bool _isEqualRule;
    private Element _conditionRuleElement;
    public Element ConditionRuleElement
    {
        get => _conditionRuleElement;
        set
        {
            _conditionRuleElement = value;
        }
    }

    private TypeRule(bool isEqualRule)
    {
        _isEqualRule = isEqualRule;
    }

    public static TypeRule Equal => new(true);
    public static TypeRule NotEqual => new(false);

    public ICondition ConstructCondition(Element target)
    {
        ArgumentNullException.ThrowIfNull(target);

        var condition = new TypeMatchCondition(target, ConditionRuleElement);
        return _isEqualRule ? condition : new ReverseCondition(condition);
    }
}

public static class RuleExtension
{
    public static IRule ValueConstructingSet(this IRule rule, Element value)
    {
        rule.ConditionRuleElement = value ?? throw new ArgumentNullException(nameof(value), "Value cannot be null.");
        return rule;
    }
}