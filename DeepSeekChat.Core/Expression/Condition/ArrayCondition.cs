using DeepSeekChat.Core.Helpers;
using System;
using System.Collections;
using System.Linq;

namespace DeepSeekChat.Core.Expression.Condition;

public class ArrayCondition : ICondition
{
    public Element Target { get; set; }

    public ArrayCondition(Element target)
    {
        Target = target ?? throw new ArgumentNullException(nameof(target));
    }
    public Element[] GetElements()
    {
        return [Target];
    }

    public bool GetResult()
    {
        var t = Target.GetValue();
        return t.GetType().GetBasedLinkTypeInterfacesWithSelf().Contains(typeof(IEnumerable));
    }
}

public class ArrayTypeCondition : ICondition
{
    public Element Target { get; set; }

    public ArrayTypeCondition(Element target)
    {
        Target = target ?? throw new ArgumentNullException(nameof(target));
    }
    public Element[] GetElements()
    {
        return [Target];
    }

    public bool GetResult()
    {
        var t = Target.GetValue() as Type ?? Type.Missing.GetType();
        return t.GetBasedLinkTypeInterfacesWithSelf().Contains(typeof(IEnumerable));
    }
}
