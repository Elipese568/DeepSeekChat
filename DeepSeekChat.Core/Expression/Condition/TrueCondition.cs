namespace DeepSeekChat.Core.Expression.Condition;

public class TrueCondition : ICondition
{
    public Element[] GetElements()
    {
        return [];
    }

    public bool GetResult()
    {
        return true;
    }
}
