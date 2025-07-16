namespace DeepSeekChat.Core.Expression.Condition;

public sealed class EqualCondition : ConditionElement
{
    Element _elementLeft = default;
    Element _elementRight = default;

    public EqualCondition(Element left, Element right) : base(left, right)
    {
    }

    public override bool GetResult()
    {
        return _elementLeft.Equals(_elementRight);
    }

    public override Element Left { get => _elementLeft; set => _elementLeft = value; }
    public override Element Right { get => _elementRight; set => _elementRight = value; }
}
