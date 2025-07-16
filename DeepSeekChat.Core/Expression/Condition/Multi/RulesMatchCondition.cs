using System.Collections.ObjectModel;
using System.Linq;

namespace DeepSeekChat.Core.Expression.Condition.Multi;

public abstract class RulesMatchCondition : Element, ICondition
{
    public abstract ReadOnlyCollection<IRule> Rules { get; }

    public virtual Element Target { get; set; }

    public Element[] GetElements()
    {
        return [Target];
    }

    public virtual bool GetResult()
    {
        return Rules.All(rule => rule.ConstructCondition(Target).GetResult());
    }
}

