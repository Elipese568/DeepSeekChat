using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DeepSeekChat.Core.Expression.Condition.Multi;

public class AllRulesMatchCondition : RulesMatchCondition
{
    private List<IRule> _rules;

    public override ReadOnlyCollection<IRule> Rules => _rules.AsReadOnly();

    public AllRulesMatchCondition()
    {
        _rules = [];
        Target = null;
    }

    public AllRulesMatchCondition(Element target)
    {
        _rules = [];
        Target = target;
    }
    public AllRulesMatchCondition(IEnumerable<IRule> rules)
    {
        _rules = new(rules);
        Target = null;
    }
    public AllRulesMatchCondition(IEnumerable<IRule> rules, Element target)
    {
        _rules = new(rules);
        Target = target;
    }

    public sealed override object GetValue()
    {
        return GetResult();
    }
}

