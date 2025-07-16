namespace DeepSeekChat.Core.Expression;

public class NullElement : Element
{
    public override object GetValue()
    {
        return null;
    }

    public readonly static NullElement Null = new();
}
