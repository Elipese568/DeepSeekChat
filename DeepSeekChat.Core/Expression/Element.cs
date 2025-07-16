namespace DeepSeekChat.Core.Expression;

public abstract class Element
{
    public abstract object GetValue();

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        if (obj.GetType().BaseType != typeof(Element)) return false;

        return GetValue().Equals((obj as Element).GetValue());
    }
}