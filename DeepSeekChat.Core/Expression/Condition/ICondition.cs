namespace DeepSeekChat.Core.Expression.Condition;

public interface ICondition
{
    public bool GetResult();
    public Element[] GetElements();
}
