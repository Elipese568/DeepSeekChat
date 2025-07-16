using System.Collections.Generic;
using System.Linq;

namespace DeepSeekChat.Core.Expression.Condition.Multi;

public class AnyRulesMatchCondition : AllRulesMatchCondition
{
    public AnyRulesMatchCondition() : base()
    {
    }
    public AnyRulesMatchCondition(Element target) : base(target)
    {
    }
    public AnyRulesMatchCondition(IEnumerable<IRule> rules) : base(rules)
    {
    }
    public AnyRulesMatchCondition(IEnumerable<IRule> rules, Element target) : base(rules, target)
    {
    }
    public override bool GetResult()
    {
        return Rules.Any(rule => rule.ConstructCondition(Target).GetResult());
    }
}
